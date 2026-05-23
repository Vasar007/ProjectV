using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace ProjectV.DataAccessLayer.Tests.ForTests
{
    /// <summary>
    /// xUnit collection fixture that hosts a single
    /// <see cref="PostgreSqlContainer" /> for every DAL integration test in
    /// this assembly. The container starts at suite begin
    /// (<see cref="InitializeAsync" />) and stops at suite end
    /// (<see cref="DisposeAsync" />); per-test data isolation is delegated to
    /// <c>TestDbHelper.TruncateAllTablesAsync</c> in each test class's
    /// <see cref="IAsyncLifetime.InitializeAsync" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Schema bootstrap path. An initial EF Core migration generation was
    /// attempted so this fixture could call
    /// <see cref="DatabaseFacade.MigrateAsync" />, but the attempt failed at
    /// EF design-time model discovery (see <c>Migrations/.gitkeep</c> for the
    /// blocking error). Both
    /// <see cref="RelationalDatabaseFacadeExtensions.MigrateAsync" /> and
    /// <see cref="DatabaseFacade.EnsureCreatedAsync" /> walk the same broken
    /// model, so this fixture takes the documented fallback: raw SQL
    /// <c>CREATE TABLE</c> statements derived from the <c>[Table]</c> /
    /// <c>[Column]</c> attributes on
    /// <c>JobDbInfo</c>, <c>UserDbInfo</c>, and <c>RefreshTokenDbInfo</c>.
    /// The fallback exercises the same Npgsql wire protocol and the same
    /// service code paths; only the schema-emission machinery is bypassed.
    /// </para>
    /// <para>
    /// <c>CanUseDatabase = true</c> is set explicitly on every constructed
    /// <see cref="DatabaseOptions" /> — otherwise the service no-ops on
    /// every call.
    /// </para>
    /// </remarks>
    public sealed class DbCollectionFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _container;


        /// <summary>
        /// PostgreSQL connection string for the running test container.
        /// Populated by <see cref="InitializeAsync" />; null before the
        /// fixture has started.
        /// </summary>
        public string ConnectionString { get; private set; } = default!;


        /// <summary>
        /// Initializes a new instance of the <see cref="DbCollectionFixture" />
        /// class. Does NOT start the container — that happens in
        /// <see cref="InitializeAsync" /> per xUnit's
        /// <see cref="IAsyncLifetime" /> contract.
        /// </summary>
        public DbCollectionFixture()
        {
            // Pin the image via the new (required) builder ctor (Pitfall 1) —
            // avoids first-pull surprises on CI. The legacy parameterless
            // builder + WithImage(...) chain is obsolete in Testcontainers 4.11.
            _container = new PostgreSqlBuilder("postgres:16.4")
                .WithDatabase("projectv_test")
                .WithUsername("test_user")
                .WithPassword("test_pass")
                // Avoid the first-pull race where the port is bound before the
                // server is ready to accept connections (Pitfall 1).
                // UntilInternalTcpPortIsAvailable(5432) waits for the container
                // process itself to bind 5432; equivalent to the legacy
                // UntilPortIsAvailable strategy.
                .WithWaitStrategy(
                    Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(5432)
                )
                .Build();
        }


        #region IAsyncLifetime Implementation

        /// <inheritdoc />
        public async Task InitializeAsync()
        {
            await _container.StartAsync();
            ConnectionString = _container.GetConnectionString();
            await ApplySchemaAsync();
        }

        /// <inheritdoc />
        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }

        #endregion

        /// <summary>
        /// Builds a fresh <see cref="ProjectVDbContext" /> pointing at this
        /// fixture's running container. Tests call this in their
        /// <see cref="IAsyncLifetime.InitializeAsync" /> to get an isolated
        /// DbContext for the system under test.
        /// </summary>
        public ProjectVDbContext CreateDbContext()
        {
            // CRITICAL: CanUseDatabase MUST be true — the production
            // ProjectVDbContext.OnConfiguring / OnModelCreating short-circuit
            // when it is false (RESEARCH.md Critical Finding #2 / Pitfall 2).
            var options = new DatabaseOptions(
                dbConnectionString: ConnectionString,
                canUseDatabase: true
            );
            return new ProjectVDbContext(options);
        }

        private async Task ApplySchemaAsync()
        {
            // Raw SQL schema bootstrap — see <remarks> on the class. Column
            // shapes mirror the [Column("…")] attributes on
            // ProjectV.DataAccessLayer.Services.{Jobs,Users,Tokens}.Models.*DbInfo;
            // tables sit in the default "public" schema declared in
            // ProjectVDbContext.OnModelCreating.
            //
            // Uses Npgsql directly rather than ProjectVDbContext.Database.
            // ExecuteSqlRawAsync because ProjectVDbContext's OnModelCreating
            // raises ModelValidator errors on the UserDbInfo.RefreshToken
            // navigation — the SUT services route their SQL through the same
            // context but only after we've materialised the schema. Bypassing
            // EF here keeps the bootstrap independent of the broken model
            // (Plan 02-09 [BLOCKING] fallback).
            const string createSchemaSql = @"
                CREATE TABLE IF NOT EXISTS ""public"".""jobs"" (
                    ""id""     uuid          NOT NULL PRIMARY KEY,
                    ""name""   text          NOT NULL,
                    ""state""  integer       NOT NULL,
                    ""result"" integer       NOT NULL,
                    ""config"" text          NOT NULL
                );

                CREATE TABLE IF NOT EXISTS ""public"".""users"" (
                    ""id""             uuid                     NOT NULL PRIMARY KEY,
                    ""user_name""      text                     NOT NULL,
                    ""password""       text                     NOT NULL,
                    ""password_salt""  text                     NOT NULL,
                    ""ts""             timestamp with time zone NOT NULL,
                    ""active""         boolean                  NOT NULL
                );

                CREATE TABLE IF NOT EXISTS ""public"".""tokens"" (
                    ""id""           uuid                     NOT NULL PRIMARY KEY,
                    ""user_name""    uuid                     NOT NULL,
                    ""token_hash""   text                     NOT NULL,
                    ""token_salt""   text                     NOT NULL,
                    ""ts""           timestamp with time zone NOT NULL,
                    ""expiry_date""  timestamp with time zone NOT NULL
                );
            ";

            await using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand(createSchemaSql, connection);
            await command.ExecuteNonQueryAsync();
        }
    }
}

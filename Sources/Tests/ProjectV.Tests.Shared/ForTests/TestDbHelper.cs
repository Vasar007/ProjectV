using Acolyte.Assertions;
using Npgsql;

namespace ProjectV.Tests.Shared.ForTests
{
    /// <summary>
    /// Utility for Testcontainers-based DB reset between test cases. Issues a
    /// <c>TRUNCATE … RESTART IDENTITY CASCADE</c> against the three production
    /// DAL tables to wipe row state without dropping the schema.
    /// Call from <see cref="IAsyncLifetime.InitializeAsync" />
    /// of each integration test class so every test starts on a clean slate.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Table names <c>"public"."jobs" / "users" / "tokens"</c> match the
    /// <c>[Table(...)]</c> attributes on
    /// <c>ProjectV.DataAccessLayer.Services.{Jobs,Users,Tokens}.Models.*DbInfo</c>
    /// and the <c>HasDefaultSchema("public")</c> declaration in
    /// <c>ProjectVDbContext.OnModelCreating</c>. Double-quoted identifiers
    /// preserve PostgreSQL case sensitivity.
    /// </para>
    /// <para>
    /// Takes a raw connection string rather than a <c>ProjectVDbContext</c>
    /// because the production context's <c>OnModelCreating</c> raises a
    /// <see cref="System.InvalidOperationException" /> on the
    /// <c>UserDbInfo.RefreshToken</c> property whenever the dependency cache
    /// is first realised — even for a TRUNCATE that never touches the model.
    /// See <c>DbCollectionFixture</c> remarks for the full rationale.
    /// Using <see cref="NpgsqlConnection" /> directly keeps the helper
    /// independent of EF Core's model validator.
    /// </para>
    /// </remarks>
    public sealed class TestDbHelper
    {
        private readonly string _connectionString;


        /// <summary>
        /// Initializes a new instance of the <see cref="TestDbHelper" /> class.
        /// </summary>
        /// <param name="connectionString">
        /// PostgreSQL connection string of the test container. Must point at
        /// the same database the SUT's <c>ProjectVDbContext</c> consumes.
        /// </param>
        public TestDbHelper(string connectionString)
        {
            _connectionString = connectionString.ThrowIfNullOrWhiteSpace(
                nameof(connectionString));
        }

        /// <summary>
        /// Resets all DAL test tables (<c>jobs</c>, <c>users</c>, <c>tokens</c>)
        /// to empty state, preserving schema and resetting any identity
        /// sequences. Use from <see cref="Xunit.IAsyncLifetime.InitializeAsync" />
        /// before constructing the system under test.
        /// </summary>
        public async Task TruncateAllTablesAsync()
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand(
                "TRUNCATE TABLE \"public\".\"jobs\", \"public\".\"users\", \"public\".\"tokens\" RESTART IDENTITY CASCADE;",
                connection);
            await command.ExecuteNonQueryAsync();
        }
    }
}

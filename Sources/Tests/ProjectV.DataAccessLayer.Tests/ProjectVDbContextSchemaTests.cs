using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Acolyte.Assertions;
using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using ProjectV.DataAccessLayer.Tests.ForTests;
using ProjectV.Tests.Shared.ForTests;
using Xunit;

namespace ProjectV.DataAccessLayer.Tests
{
    /// <summary>
    /// Integration test asserting that the schema applied by
    /// <see cref="DbCollectionFixture" /> exposes the three expected DAL
    /// tables in the <c>public</c> schema. Per 02-09 Task 1's [BLOCKING]
    /// fallback, the schema is bootstrapped via raw SQL (see
    /// <c>DbCollectionFixture.ApplySchemaAsync</c>) rather than EF Core
    /// migrations — this test verifies the bootstrap is wired correctly.
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("RequiresDocker", "true")]
    [Collection(DbCollection.Name)]
    public sealed class ProjectVDbContextSchemaTests : BaseMockTest, IAsyncLifetime
    {
        private readonly DbCollectionFixture _db;

        private ProjectVDbContext _context = default!;


        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ProjectVDbContextSchemaTests" /> class.
        /// </summary>
        public ProjectVDbContextSchemaTests(DbCollectionFixture db)
        {
            _db = db.ThrowIfNull(nameof(db));
        }


        #region IAsyncLifetime Implementation

        /// <inheritdoc />
        public Task InitializeAsync()
        {
            _context = _db.CreateDbContext();
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        #endregion

        [Fact]
        public async Task SchemaAfterBootstrapContainsAllExpectedTables()
        {
            // Arrange.
            var expectedTables = new[] { "jobs", "users", "tokens" };

            // Act.
            HashSet<string> actualTables = await QueryPublicSchemaTableNamesAsync();

            // Assert.
            actualTables.Should().Contain(expectedTables,
                "the DbCollectionFixture must materialise the production " +
                "DAL tables (jobs / users / tokens) in the `public` schema " +
                "of the Testcontainers PostgreSQL instance.");
        }

        [Fact]
        public async Task CanUseDbIsTrueOnFixtureBackedContext()
        {
            // Arrange. / Act.
            bool actualValue = _context.CanUseDb();

            // Assert.
            actualValue.Should().BeTrue(
                "every DbContext produced by DbCollectionFixture must carry " +
                "CanUseDatabase=true — Pitfall 2 in 02-RESEARCH.md.");

            // Sanity check: round-trip a trivial query to confirm the Npgsql
            // connection actually opens against the container.
            await using DbConnection connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();
            connection.State.Should().Be(System.Data.ConnectionState.Open);
        }

        private async Task<HashSet<string>> QueryPublicSchemaTableNamesAsync()
        {
            const string sql =
                @"SELECT table_name FROM information_schema.tables
                  WHERE table_schema = 'public' AND table_type = 'BASE TABLE';";

            await using DbConnection connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            await using DbCommand command = connection.CreateCommand();
            command.CommandText = sql;
            await using DbDataReader reader = await command.ExecuteReaderAsync();

            var result = new HashSet<string>();
            while (await reader.ReadAsync())
            {
                result.Add(reader.GetString(0));
            }
            return result;
        }
    }
}

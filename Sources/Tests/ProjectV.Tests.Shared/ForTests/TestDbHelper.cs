using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.EntityFrameworkCore;
using ProjectV.DataAccessLayer;

namespace ProjectV.Tests.Shared.ForTests
{
    /// <summary>
    /// Utility for Testcontainers-based DB reset between test cases. Issues a
    /// <c>TRUNCATE … RESTART IDENTITY CASCADE</c> against the three production
    /// DAL tables to wipe row state without dropping the schema (Decision D-11
    /// in 02-CONTEXT.md). Call from <see cref="IAsyncLifetime.InitializeAsync" />
    /// of each integration test class so every test starts on a clean slate.
    /// </summary>
    /// <remarks>
    /// Table names <c>"public"."jobs" / "users" / "tokens"</c> match the
    /// <c>[Table(...)]</c> attributes on
    /// <c>ProjectV.DataAccessLayer.Services.{Jobs,Users,Tokens}.Models.*DbInfo</c>
    /// and the <c>HasDefaultSchema("public")</c> declaration in
    /// <see cref="ProjectVDbContext.OnModelCreating" />. Double-quoted
    /// identifiers preserve PostgreSQL case sensitivity.
    /// </remarks>
    public sealed class TestDbHelper
    {
        private readonly ProjectVDbContext _context;


        /// <summary>
        /// Initializes a new instance of the <see cref="TestDbHelper" /> class.
        /// </summary>
        /// <param name="context">
        /// A <see cref="ProjectVDbContext" /> configured to point at the
        /// Testcontainers PostgreSQL instance with
        /// <c>DatabaseOptions.CanUseDatabase = true</c>.
        /// </param>
        public TestDbHelper(ProjectVDbContext context)
        {
            _context = context.ThrowIfNull(nameof(context));
        }

        /// <summary>
        /// Resets all DAL test tables (<c>jobs</c>, <c>users</c>, <c>tokens</c>)
        /// to empty state, preserving schema and resetting any identity
        /// sequences. Detaches every tracked entity first so a subsequent
        /// <see cref="DbContext.SaveChangesAsync(System.Threading.CancellationToken)" />
        /// does not raise <c>DbUpdateConcurrencyException</c> on a stale
        /// reference (reference D-32 in 02-CONTEXT.md).
        /// </summary>
        public async Task TruncateAllTablesAsync()
        {
            _context.ChangeTracker.Clear();
            await _context.Database.ExecuteSqlRawAsync(
                "TRUNCATE TABLE \"public\".\"jobs\", \"public\".\"users\", \"public\".\"tokens\" RESTART IDENTITY CASCADE;"
            );
        }
    }
}

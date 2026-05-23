using System.Threading.Tasks;
using Acolyte.Assertions;
using AwesomeAssertions;
using ProjectV.DataAccessLayer.Services.Jobs;
using ProjectV.DataAccessLayer.Tests.ForTests;
using ProjectV.Models.Internal.Jobs;
using ProjectV.Tests.Shared.ForTests;
using ProjectV.Tests.Shared.Helpers.Generators.DataAccessLayer;
using Xunit;

namespace ProjectV.DataAccessLayer.Tests.Services.Jobs
{
    /// <summary>
    /// Integration tests for <see cref="DatabaseJobInfoService" /> against a
    /// real Testcontainers PostgreSQL instance via
    /// <see cref="DbCollectionFixture" /> — exercises Add/Find/Update on the
    /// production Npgsql pipeline.
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("RequiresDocker", "true")]
    [Collection(DbCollection.Name)]
    public sealed class DatabaseJobInfoServiceTests : BaseMockTest, IAsyncLifetime
    {
        private readonly DbCollectionFixture _db;
        private readonly JobInfoGenerator _generator;

        private ProjectVDbContext _context = default!;
        private TestDbHelper _dbHelper = default!;
        private DatabaseJobInfoService _sut = default!;


        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DatabaseJobInfoServiceTests" /> class. The
        /// <see cref="DbCollectionFixture" /> is injected by xUnit's collection
        /// fixture machinery (see <see cref="DbCollection" />).
        /// </summary>
        public DatabaseJobInfoServiceTests(DbCollectionFixture db)
        {
            _db = db.ThrowIfNull(nameof(db));
            _generator = new JobInfoGenerator();
        }


        #region IAsyncLifetime Implementation

        /// <inheritdoc />
        public async Task InitializeAsync()
        {
            _dbHelper = new TestDbHelper(_db.ConnectionString);
            await _dbHelper.TruncateAllTablesAsync();

            _context = _db.CreateDbContext();
            _sut = new DatabaseJobInfoService(_context, new DataAccessLayerMapper());
        }

        /// <inheritdoc />
        public async Task DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        #endregion

        [Fact]
        public async Task AddAsyncWithValidJobInfoReturnsSavedRow()
        {
            // Arrange.
            JobInfo jobInfo = _generator.GenerateJobInfo();

            // Act.
            int actualValue = await _sut.AddAsync(jobInfo);

            // Assert.
            actualValue.Should().BeGreaterThan(0,
                "DatabaseJobInfoService.AddAsync should return the number of " +
                "rows persisted (1 in the happy path).");
        }

        [Fact]
        public async Task FindByIdAsyncAfterAddReturnsEquivalentJob()
        {
            // Arrange.
            JobInfo expected = _generator.GenerateJobInfo();
            await _sut.AddAsync(expected);

            // Act.
            JobInfo? actualValue = await _sut.FindByIdAsync(expected.Id);

            // Assert.
            actualValue.Should().NotBeNull();
            actualValue!.Id.Should().Be(expected.Id);
            actualValue.Name.Should().Be(expected.Name);
            actualValue.State.Should().Be(expected.State);
            actualValue.Result.Should().Be(expected.Result);
            actualValue.Config.Should().Be(expected.Config);
        }

        [Fact]
        public async Task UpdateAsyncWithExistingJobPersistsChanges()
        {
            // Arrange.
            JobInfo original = _generator.GenerateJobInfo();
            await _sut.AddAsync(original);

            var mutated = new JobInfo(
                id: original.Id,
                name: original.Name,
                state: original.State + 1,
                result: original.Result + 1,
                config: original.Config
            );

            // Detach the tracked entity so Update does not fight an in-memory copy.
            _context.ChangeTracker.Clear();

            // Act.
            int rowsAffected = await _sut.UpdateAsync(mutated);
            JobInfo? reread = await _sut.FindByIdAsync(original.Id);

            // Assert.
            rowsAffected.Should().BeGreaterThan(0);
            reread.Should().NotBeNull();
            reread!.State.Should().Be(mutated.State);
            reread.Result.Should().Be(mutated.Result);
        }
    }
}

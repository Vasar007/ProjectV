using System.Threading.Tasks;
using Acolyte.Assertions;
using AwesomeAssertions;
using ProjectV.DataAccessLayer.Services.Users;
using ProjectV.DataAccessLayer.Tests.ForTests;
using ProjectV.Models.Users;
using ProjectV.Tests.Shared.ForTests;
using ProjectV.Tests.Shared.Helpers.Generators.DataAccessLayer;
using Xunit;

namespace ProjectV.DataAccessLayer.Tests.Services.Users
{
    /// <summary>
    /// Integration tests for <see cref="DatabaseUserInfoService" /> against a
    /// real Testcontainers PostgreSQL instance — exercises Add /
    /// FindById / FindByUserName on the production Npgsql pipeline.
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("RequiresDocker", "true")]
    [Collection(DbCollection.Name)]
    public sealed class DatabaseUserInfoServiceTests : BaseMockTest, IAsyncLifetime
    {
        private readonly DbCollectionFixture _db;
        private readonly UserInfoGenerator _generator;

        private ProjectVDbContext _context = default!;
        private TestDbHelper _dbHelper = default!;
        private DatabaseUserInfoService _sut = default!;


        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DatabaseUserInfoServiceTests" /> class.
        /// </summary>
        public DatabaseUserInfoServiceTests(DbCollectionFixture db)
        {
            _db = db.ThrowIfNull(nameof(db));
            _generator = new UserInfoGenerator();
        }


        #region IAsyncLifetime Implementation

        /// <inheritdoc />
        public async Task InitializeAsync()
        {
            _dbHelper = new TestDbHelper(_db.ConnectionString);
            await _dbHelper.TruncateAllTablesAsync();

            _context = _db.CreateDbContext();
            _sut = new DatabaseUserInfoService(_context, new DataAccessLayerMapper());
        }

        /// <inheritdoc />
        public async Task DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        #endregion

        [Fact]
        public async Task AddAsyncWithValidUserReturnsSavedRow()
        {
            // Arrange.
            UserInfo userInfo = _generator.GenerateUserInfo();

            // Act.
            int actualValue = await _sut.AddAsync(userInfo);

            // Assert.
            actualValue.Should().BeGreaterThan(0,
                "DatabaseUserInfoService.AddAsync should return the count of " +
                "rows persisted (1 in the happy path).");
        }

        [Fact]
        public async Task FindByIdAsyncAfterAddReturnsEquivalentUser()
        {
            // Arrange.
            UserInfo expected = _generator.GenerateUserInfo();
            await _sut.AddAsync(expected);

            // Act.
            UserInfo? actualValue = await _sut.FindByIdAsync(expected.Id);

            // Assert.
            actualValue.Should().NotBeNull();
            actualValue!.Id.Should().Be(expected.Id);
            actualValue.UserName.Should().Be(expected.UserName);
            actualValue.PasswordSalt.Should().Be(expected.PasswordSalt);
            actualValue.Active.Should().Be(expected.Active);
        }

        [Fact]
        public async Task FindByUserNameAsyncAfterAddReturnsUser()
        {
            // Arrange.
            UserInfo expected = _generator.GenerateUserInfo();
            await _sut.AddAsync(expected);

            // Act.
            UserInfo? actualValue = await _sut.FindByUserNameAsync(expected.UserName);

            // Assert.
            actualValue.Should().NotBeNull();
            actualValue!.Id.Should().Be(expected.Id);
            actualValue.UserName.Should().Be(expected.UserName);
        }
    }
}

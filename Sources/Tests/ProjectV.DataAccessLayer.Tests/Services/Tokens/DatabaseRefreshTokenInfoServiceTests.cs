using System;
using System.Threading.Tasks;
using Acolyte.Assertions;
using AwesomeAssertions;
using ProjectV.DataAccessLayer.Services.Tokens;
using ProjectV.DataAccessLayer.Tests.ForTests;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.Users;
using ProjectV.Tests.Shared.ForTests;
using ProjectV.Tests.Shared.Helpers.Generators.DataAccessLayer;
using ProjectV.Tests.Shared.Helpers.Generators.Models;
using Xunit;

namespace ProjectV.DataAccessLayer.Tests.Services.Tokens
{
    /// <summary>
    /// Integration tests for <see cref="DatabaseRefreshTokenInfoService" />
    /// against a real Testcontainers PostgreSQL instance — exercises
    /// Add / FindById / FindByUserId / expiry round-trip on the production
    /// Npgsql pipeline.
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("RequiresDocker", "true")]
    [Collection(DbCollection.Name)]
    public sealed class DatabaseRefreshTokenInfoServiceTests : BaseMockTest, IAsyncLifetime
    {
        private readonly DbCollectionFixture _db;
        private readonly RefreshTokenInfoGenerator _generator;

        private ProjectVDbContext _context = default!;
        private TestDbHelper _dbHelper = default!;
        private DatabaseRefreshTokenInfoService _sut = default!;


        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DatabaseRefreshTokenInfoServiceTests" /> class.
        /// </summary>
        public DatabaseRefreshTokenInfoServiceTests(DbCollectionFixture db)
        {
            _db = db.ThrowIfNull(nameof(db));
            _generator = new RefreshTokenInfoGenerator(new UserIdGenerator());
        }


        #region IAsyncLifetime Implementation

        /// <inheritdoc />
        public async Task InitializeAsync()
        {
            _dbHelper = new TestDbHelper(_db.ConnectionString);
            await _dbHelper.TruncateAllTablesAsync();

            _context = _db.CreateDbContext();
            _sut = new DatabaseRefreshTokenInfoService(_context, new DataAccessLayerMapper());
        }

        /// <inheritdoc />
        public async Task DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        #endregion

        [Fact]
        public async Task AddAsyncWithValidTokenPersistsRow()
        {
            // Arrange.
            RefreshTokenInfo tokenInfo = _generator.GenerateRefreshTokenInfo();

            // Act.
            int actualValue = await _sut.AddAsync(tokenInfo);

            // Assert.
            actualValue.Should().BeGreaterThan(0,
                "DatabaseRefreshTokenInfoService.AddAsync should return the " +
                "count of rows persisted (1 in the happy path).");
        }

        [Fact]
        public async Task FindByIdAsyncAfterAddReturnsTokenWithExpectedExpiry()
        {
            // Arrange.
            var creation = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime expiry = creation.AddDays(7);
            RefreshTokenInfo expected = _generator.GenerateRefreshTokenInfo(
                creationTimeUtc: creation,
                expiryDateUtc: expiry
            );
            await _sut.AddAsync(expected);

            // Act.
            RefreshTokenInfo? actualValue = await _sut.FindByIdAsync(expected.Id);

            // Assert.
            actualValue.Should().NotBeNull();
            actualValue!.Id.Should().Be(expected.Id);
            actualValue.UserId.Should().Be(expected.UserId);
            actualValue.TokenSalt.Should().Be(expected.TokenSalt);
            // Postgres `timestamp with time zone` round-trips as Utc.
            actualValue.ExpiryDateUtc.Should().BeCloseTo(
                expiry, precision: TimeSpan.FromMilliseconds(1));
        }

        /// <summary>
        /// Exercises the raw-Guid comparison fix: the service must look up
        /// tokens by user id through the EF-translatable scalar column path
        /// (not via the WrappedUserId computed property, which EF cannot lift
        /// into SQL). Without this test the fix has zero integration coverage;
        /// a regression that reintroduces
        /// `token.WrappedUserId == userId` in the predicate would crash at
        /// runtime instead of being caught here. Assertions extend beyond
        /// id round-trip to cover the credential fields (TokenHash /
        /// TokenSalt / ExpiryDate) — a regression that returned the wrong
        /// token row for a user (e.g. predicate inversion, ordering bug)
        /// would slip past an id-only assertion if any other token row
        /// existed; the full-field round-trip rules that out.
        /// </summary>
        [Fact]
        public async Task FindByUserIdAsyncAfterAddReturnsTokenWithExpectedFields()
        {
            // Arrange.
            var creation = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime expiry = creation.AddDays(14);
            RefreshTokenInfo expected = _generator.GenerateRefreshTokenInfo(
                creationTimeUtc: creation,
                expiryDateUtc: expiry
            );
            await _sut.AddAsync(expected);

            // Act.
            RefreshTokenInfo? actualValue = await _sut.FindByUserIdAsync(expected.UserId);

            // Assert.
            actualValue.Should().NotBeNull();
            actualValue!.Id.Should().Be(expected.Id);
            actualValue.UserId.Should().Be(expected.UserId);
            actualValue.TokenHash.Should().Be(expected.TokenHash);
            actualValue.TokenSalt.Should().Be(expected.TokenSalt);
            // Postgres `timestamp with time zone` round-trips as Utc.
            actualValue.ExpiryDateUtc.Should().BeCloseTo(
                expiry, precision: TimeSpan.FromMilliseconds(1));
        }

        /// <summary>
        /// Covers the null-return branch of <c>FindByUserIdAsync</c> — the
        /// service returns <c>null</c> when no token row matches the supplied
        /// user id. A regression that altered the predicate (e.g. wrong field
        /// comparison, inverted boolean, accidental cross-row match) would
        /// surface here as a non-null value returned for a generated-but-not-
        /// inserted user id.
        /// </summary>
        [Fact]
        public async Task FindByUserIdAsyncForUnknownUserReturnsNull()
        {
            // Arrange — generate a user id but do NOT insert any token row
            // for it. The user id space is `Guid.NewGuid()`-backed so the
            // collision probability with any pre-existing test fixture data
            // is effectively zero (the `TruncateAllTablesAsync` step in
            // InitializeAsync also rules out leftover rows from earlier
            // tests within this collection).
            UserId unknownUserId = new UserIdGenerator().GenerateUserId();

            // Act.
            RefreshTokenInfo? actualValue = await _sut.FindByUserIdAsync(unknownUserId);

            // Assert.
            actualValue.Should().BeNull(
                "FindByUserIdAsync must return null when no token row " +
                "exists for the supplied user id");
        }

        /// <summary>
        /// Multi-row filter integration test for <c>FindByUserIdAsync</c>.
        /// Inserts two tokens for two distinct users and asserts that
        /// <c>FindByUserIdAsync(userA.Id)</c> returns tokenA (not tokenB).
        /// </summary>
        /// <remarks>
        /// The pre-existing happy-path test
        /// (<see cref="FindByUserIdAsyncAfterAddReturnsTokenWithExpectedFields" />)
        /// operates on a single-row table — it would still pass even if the
        /// EF-translated WHERE clause were a no-op (or the predicate were
        /// inverted, or the comparison column were swapped) because there is
        /// only one row that could be returned. This multi-row variant
        /// exercises the actual filter: with two candidate rows in the
        /// table, only the row whose <c>user_name</c> column matches the
        /// supplied user id is allowed to surface. A regression that broke
        /// the predicate would return the wrong token row here.
        /// </remarks>
        [Fact]
        public async Task FindByUserIdAsyncWithMultipleUsersReturnsOnlyMatchingRow()
        {
            // Arrange — insert two tokens for two distinct users. The
            // RefreshTokenInfoGenerator emits a fresh Guid.NewGuid()-backed
            // UserId on each call, so tokenA.UserId != tokenB.UserId with
            // overwhelming probability.
            RefreshTokenInfo tokenA = _generator.GenerateRefreshTokenInfo();
            RefreshTokenInfo tokenB = _generator.GenerateRefreshTokenInfo();
            tokenA.UserId.Should().NotBe(tokenB.UserId,
                "the multi-row test requires two distinct user ids to be " +
                "meaningful — generator-level guarantee, asserted defensively");
            await _sut.AddAsync(tokenA);
            await _sut.AddAsync(tokenB);

            // Act.
            RefreshTokenInfo? actualValue = await _sut.FindByUserIdAsync(tokenA.UserId);

            // Assert — must surface tokenA, must NOT surface tokenB.
            actualValue.Should().NotBeNull();
            actualValue!.Id.Should().Be(tokenA.Id);
            actualValue.UserId.Should().Be(tokenA.UserId);
            actualValue.Id.Should().NotBe(tokenB.Id,
                "the predicate must filter — returning tokenB here would " +
                "indicate a broken WHERE clause");
        }
    }
}

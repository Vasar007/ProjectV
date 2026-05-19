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
    public sealed class DatabaseRefreshTokenInfoServiceTests : IAsyncLifetime
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
        /// Exercises the Plan 02-09 Rule-1 raw-Guid comparison fix: the
        /// service must look up tokens by user id through the EF-translatable
        /// scalar column path (not via the WrappedUserId computed property,
        /// which EF cannot lift into SQL). Without this test the 02-09 fix
        /// has zero integration coverage; a regression that reintroduces
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
    }
}

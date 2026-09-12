using Acolyte.Assertions;
using ProjectV.Models.Authorization;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.Users;
using ProjectV.Tests.Shared.Helpers.Generators.Models;

namespace ProjectV.Tests.Shared.Helpers.Generators.DataAccessLayer
{
    /// <summary>
    /// Generator for <see cref="RefreshTokenInfo" /> test data. Follows the
    /// <c>Create(...)</c> / <c>Generate(...)</c> twin pattern:
    /// <list type="bullet">
    ///     <item>
    ///         <description><c>Create*</c> — every argument is explicit.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>Generate*</c> — every argument is optional;
    ///         unspecified values are filled with random valid values,
    ///         unique per call.</description>
    ///     </item>
    /// </list>
    /// </summary>
    public sealed class RefreshTokenInfoGenerator
    {
        /// <summary>
        /// Singleton instance for convenient access.
        /// </summary>
        public static RefreshTokenInfoGenerator Instance { get; } = new();

        private readonly UserIdGenerator _userIdGenerator;


        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="RefreshTokenInfoGenerator" /> class.
        /// </summary>
        public RefreshTokenInfoGenerator()
        {
            _userIdGenerator = UserIdGenerator.Instance;
        }

        /// <summary>
        /// Creates a <see cref="RefreshTokenInfo" /> with every field supplied
        /// explicitly by the caller.
        /// </summary>
        /// <param name="id">Token identifier — must be specified.</param>
        /// <param name="userId">Owning user identifier — must be specified.</param>
        /// <param name="tokenHash">Token hash — must not be null/whitespace.</param>
        /// <param name="tokenSalt">Token salt — must not be null.</param>
        /// <param name="creationTimeUtc">Token creation timestamp (UTC).</param>
        /// <param name="expiryDateUtc">Token expiry timestamp (UTC).</param>
        /// <returns>A new <see cref="RefreshTokenInfo" /> instance.</returns>
        public RefreshTokenInfo CreateRefreshTokenInfo(
            RefreshTokenId id,
            UserId userId,
            Password tokenHash,
            string tokenSalt,
            DateTime creationTimeUtc,
            DateTime expiryDateUtc)
        {
            tokenSalt.ThrowIfNull(nameof(tokenSalt));

            return new RefreshTokenInfo(
                id: id,
                userId: userId,
                tokenHash: tokenHash,
                tokenSalt: tokenSalt,
                creationTimeUtc: creationTimeUtc,
                expiryDateUtc: expiryDateUtc
            );
        }

        /// <summary>
        /// Generates a <see cref="RefreshTokenInfo" /> filling any unspecified
        /// field with a random valid value.
        /// </summary>
        /// <param name="id">Optional token identifier.</param>
        /// <param name="userId">Optional owning user identifier.</param>
        /// <param name="tokenHash">Optional token hash.</param>
        /// <param name="tokenSalt">Optional token salt.</param>
        /// <param name="creationTimeUtc">Optional creation timestamp.</param>
        /// <param name="expiryDateUtc">Optional expiry timestamp.</param>
        /// <returns>A new <see cref="RefreshTokenInfo" /> instance.</returns>
        public RefreshTokenInfo GenerateRefreshTokenInfo(
            RefreshTokenId? id = null,
            UserId? userId = null,
            Password? tokenHash = null,
            string? tokenSalt = null,
            DateTime? creationTimeUtc = null,
            DateTime? expiryDateUtc = null)
        {
            DateTime creation = creationTimeUtc ?? GenerateCreationTimeUtc();
            return CreateRefreshTokenInfo(
                id: id ?? GenerateId(),
                userId: userId ?? _userIdGenerator.GenerateUserId(),
                tokenHash: tokenHash ?? GenerateTokenHash(),
                tokenSalt: tokenSalt ?? GenerateTokenSalt(),
                creationTimeUtc: creation,
                expiryDateUtc: expiryDateUtc ?? creation.AddDays(7)
            );
        }

        /// <summary>
        /// Generates a fresh <see cref="RefreshTokenId" /> from a new GUID.
        /// </summary>
        public RefreshTokenId GenerateId()
        {
            return RefreshTokenId.Wrap(Guid.NewGuid());
        }

        /// <summary>
        /// Generates a unique <see cref="Password" /> for use as a
        /// token hash, with a GUID-derived suffix.
        /// </summary>
        public Password GenerateTokenHash()
        {
            return Password.Wrap($"token-{Guid.NewGuid():N}");
        }

        /// <summary>
        /// Generates a unique token salt with a GUID-derived suffix.
        /// </summary>
        public string GenerateTokenSalt()
        {
            return $"token-salt-{Guid.NewGuid():N}";
        }

        /// <summary>
        /// Generates a random UTC creation timestamp anchored at
        /// 2020-01-01 plus a random number of seconds within one year.
        /// </summary>
        public DateTime GenerateCreationTimeUtc()
        {
            int offsetSeconds = Random.Shared.Next(0, 365 * 24 * 60 * 60);
            return new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(offsetSeconds);
        }
    }
}

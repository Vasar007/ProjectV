using Acolyte.Assertions;
using ProjectV.Models.Authorization;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.Users;
using ProjectV.Tests.Shared.Helpers.Generators.Models;

namespace ProjectV.Tests.Shared.Helpers.Generators.DataAccessLayer
{
    /// <summary>
    /// Generator for <see cref="UserInfo" /> test data. Follows the
    /// <c>Create(...)</c> / <c>Generate(...)</c> twin pattern (Decision D-34):
    /// <list type="bullet">
    ///     <item>
    ///         <description><c>Create*</c> — every argument is explicit; the
    ///         caller is responsible for the resulting <see cref="UserInfo" />
    ///         being valid.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>Generate*</c> — every argument is optional;
    ///         unspecified values come from deterministic helpers (seeded
    ///         <see cref="Random" /> seed 42 per Specifics §5 + GUIDs).</description>
    ///     </item>
    /// </list>
    /// </summary>
    public sealed class UserInfoGenerator
    {
        private static readonly Random _random = new Random(Seed: 42);

        private readonly UserIdGenerator _userIdGenerator;


        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfoGenerator" />
        /// class with a default <see cref="UserIdGenerator" />.
        /// </summary>
        public UserInfoGenerator()
            : this(new UserIdGenerator())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfoGenerator" />
        /// class with a caller-supplied <see cref="UserIdGenerator" />.
        /// </summary>
        /// <param name="userIdGenerator">Generator for the <c>id</c> field.</param>
        public UserInfoGenerator(UserIdGenerator userIdGenerator)
        {
            _userIdGenerator = userIdGenerator.ThrowIfNull(nameof(userIdGenerator));
        }

        /// <summary>
        /// Creates a <see cref="UserInfo" /> with every field supplied
        /// explicitly by the caller.
        /// </summary>
        /// <param name="id">User identifier — must be specified.</param>
        /// <param name="userName">User name — must not be null/whitespace.</param>
        /// <param name="password">Password value — must not be null/whitespace.</param>
        /// <param name="passwordSalt">Password salt — must not be null/whitespace.</param>
        /// <param name="creationTimeUtc">Creation timestamp (UTC).</param>
        /// <param name="active">Whether the account is active.</param>
        /// <param name="refreshToken">Optional refresh token — pass <c>null</c> for no token.</param>
        /// <returns>A new <see cref="UserInfo" /> instance.</returns>
        public UserInfo CreateUserInfo(
            UserId id,
            UserName userName,
            Password password,
            string passwordSalt,
            DateTime creationTimeUtc,
            bool active,
            RefreshTokenInfo? refreshToken)
        {
            passwordSalt.ThrowIfNullOrWhiteSpace(nameof(passwordSalt));

            return new UserInfo(
                id: id,
                userName: userName,
                password: password,
                passwordSalt: passwordSalt,
                creationTimeUtc: creationTimeUtc,
                active: active,
                refreshToken: refreshToken
            );
        }

        /// <summary>
        /// Generates a <see cref="UserInfo" /> filling any unspecified field
        /// with a deterministic value.
        /// </summary>
        /// <param name="id">Optional user identifier.</param>
        /// <param name="userName">Optional user name.</param>
        /// <param name="password">Optional password value.</param>
        /// <param name="passwordSalt">Optional password salt.</param>
        /// <param name="creationTimeUtc">Optional creation timestamp.</param>
        /// <param name="active">Optional active flag (defaults to <c>true</c> when omitted).</param>
        /// <param name="refreshToken">Optional refresh token — pass <c>null</c> (default) for no token.</param>
        /// <returns>A new <see cref="UserInfo" /> instance.</returns>
        public UserInfo GenerateUserInfo(
            UserId? id = null,
            UserName? userName = null,
            Password? password = null,
            string? passwordSalt = null,
            DateTime? creationTimeUtc = null,
            bool? active = null,
            RefreshTokenInfo? refreshToken = null)
        {
            return CreateUserInfo(
                id: id ?? GenerateId(),
                userName: userName ?? GenerateUserName(),
                password: password ?? GeneratePassword(),
                passwordSalt: passwordSalt ?? GeneratePasswordSalt(),
                creationTimeUtc: creationTimeUtc ?? GenerateCreationTimeUtc(),
                active: active ?? true,
                refreshToken: refreshToken
            );
        }

        /// <summary>
        /// Generates a fresh <see cref="UserId" /> via the underlying
        /// <see cref="UserIdGenerator" />.
        /// </summary>
        public UserId GenerateId()
        {
            return _userIdGenerator.GenerateUserId();
        }

        /// <summary>
        /// Generates a deterministic <see cref="UserName" /> with a
        /// GUID-derived suffix.
        /// </summary>
        public UserName GenerateUserName()
        {
            return UserName.Wrap($"user-{Guid.NewGuid():N}");
        }

        /// <summary>
        /// Generates a deterministic <see cref="Password" /> with a
        /// GUID-derived suffix.
        /// </summary>
        public Password GeneratePassword()
        {
            return Password.Wrap($"pwd-{Guid.NewGuid():N}");
        }

        /// <summary>
        /// Generates a deterministic password salt with a GUID-derived suffix.
        /// </summary>
        public string GeneratePasswordSalt()
        {
            return $"salt-{Guid.NewGuid():N}";
        }

        /// <summary>
        /// Generates a deterministic UTC creation timestamp. Anchored at the
        /// epoch + a seeded number of seconds to keep value ranges stable
        /// across test runs.
        /// </summary>
        public DateTime GenerateCreationTimeUtc()
        {
            int offsetSeconds = _random.Next(0, 365 * 24 * 60 * 60);
            return new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(offsetSeconds);
        }
    }
}

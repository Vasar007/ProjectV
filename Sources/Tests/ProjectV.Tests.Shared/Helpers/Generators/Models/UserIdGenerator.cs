using Acolyte.Assertions;
using ProjectV.Models.Users;

namespace ProjectV.Tests.Shared.Helpers.Generators.Models
{
    /// <summary>
    /// Generator for <see cref="UserId" /> test data. Follows the
    /// <c>Create(...)</c> / <c>Generate(...)</c> twin pattern (Decision D-34):
    /// <list type="bullet">
    ///     <item>
    ///         <description><c>Create*</c> — every argument is explicit; the
    ///         caller is responsible for the resulting <see cref="UserId" />
    ///         being valid.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>Generate*</c> — every argument is optional;
    ///         unspecified values are filled with a freshly-generated
    ///         <see cref="Guid" /> string (Guids are globally unique — no
    ///         seeded <see cref="Random" /> needed for this value
    ///         object).</description>
    ///     </item>
    /// </list>
    /// </summary>
    public sealed class UserIdGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserIdGenerator" />
        /// class.
        /// </summary>
        public UserIdGenerator()
        {
        }

        /// <summary>
        /// Creates a <see cref="UserId" /> from an explicit raw GUID string
        /// via <see cref="UserId.Parse(string)" />.
        /// </summary>
        /// <param name="rawId">
        /// Raw GUID string. Must be a parseable, non-empty GUID; empty GUIDs
        /// are rejected by <see cref="UserId.Wrap(Guid)" />.
        /// </param>
        /// <returns>A new <see cref="UserId" /> instance.</returns>
        public UserId CreateUserId(string rawId)
        {
            rawId.ThrowIfNullOrEmpty(nameof(rawId));

            return UserId.Parse(rawId);
        }

        /// <summary>
        /// Generates a <see cref="UserId" />, optionally seeded with a
        /// caller-supplied raw GUID string. When <paramref name="rawId" /> is
        /// <c>null</c>, a fresh <see cref="Guid" /> is used.
        /// </summary>
        /// <param name="rawId">Optional raw GUID string.</param>
        /// <returns>A new <see cref="UserId" /> instance.</returns>
        public UserId GenerateUserId(string? rawId = null)
        {
            return CreateUserId(rawId ?? GenerateRawId());
        }

        /// <summary>
        /// Generates a fresh raw GUID string (no dashes) suitable for feeding
        /// into <see cref="UserId.Parse(string)" />.
        /// </summary>
        public string GenerateRawId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}

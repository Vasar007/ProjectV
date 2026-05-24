using Acolyte.Assertions;
using ProjectV.Models.Internal.Jobs;

namespace ProjectV.Tests.Shared.Helpers.Generators.Models
{
    /// <summary>
    /// Generator for <see cref="JobId" /> test data. Follows the
    /// <c>Create(...)</c> / <c>Generate(...)</c> twin pattern:
    /// <list type="bullet">
    ///     <item>
    ///         <description><c>Create*</c> — every argument is explicit; the
    ///         caller is responsible for the resulting <see cref="JobId" />
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
    public sealed class JobIdGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobIdGenerator" />
        /// class.
        /// </summary>
        public JobIdGenerator()
        {
        }

        /// <summary>
        /// Creates a <see cref="JobId" /> from an explicit raw GUID string
        /// via <see cref="JobId.Parse(string)" />.
        /// </summary>
        /// <param name="rawId">
        /// Raw GUID string. Must be a parseable, non-empty GUID; empty GUIDs
        /// are rejected by <see cref="JobId.Wrap(Guid)" />.
        /// </param>
        /// <returns>A new <see cref="JobId" /> instance.</returns>
        public JobId CreateJobId(string rawId)
        {
            rawId.ThrowIfNullOrEmpty(nameof(rawId));

            return JobId.Parse(rawId);
        }

        /// <summary>
        /// Generates a <see cref="JobId" />, optionally seeded with a
        /// caller-supplied raw GUID string. When <paramref name="rawId" /> is
        /// <c>null</c>, a fresh <see cref="Guid" /> is used.
        /// </summary>
        /// <param name="rawId">Optional raw GUID string.</param>
        /// <returns>A new <see cref="JobId" /> instance.</returns>
        public JobId GenerateJobId(string? rawId = null)
        {
            return CreateJobId(rawId ?? GenerateRawId());
        }

        /// <summary>
        /// Generates a fresh raw GUID string (no dashes) suitable for feeding
        /// into <see cref="JobId.Parse(string)" />.
        /// </summary>
        public string GenerateRawId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}

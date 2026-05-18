using Acolyte.Assertions;
using ProjectV.Models.Data;

namespace ProjectV.Tests.Shared.Helpers.Generators.Models
{
    /// <summary>
    /// Generator for <see cref="BasicInfo" /> test data. Follows the
    /// <c>Create(...)</c> / <c>Generate(...)</c> twin pattern (Decision D-34):
    /// <list type="bullet">
    ///     <item>
    ///         <description><c>Create*</c> — every argument is explicit; the
    ///         caller is responsible for the resulting <see cref="BasicInfo" />
    ///         being valid.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>Generate*</c> — every argument is optional;
    ///         unspecified values come from a deterministic seeded
    ///         <see cref="Random" /> (seed 42 per Specifics §5).</description>
    ///     </item>
    /// </list>
    /// </summary>
    public sealed class BasicInfoGenerator
    {
        private static readonly Random _random = new Random(Seed: 42);

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicInfoGenerator" />
        /// class.
        /// </summary>
        public BasicInfoGenerator()
        {
        }

        /// <summary>
        /// Creates a <see cref="BasicInfo" /> with every field supplied
        /// explicitly by the caller.
        /// </summary>
        /// <param name="thingId">Unique identifier.</param>
        /// <param name="title">Title — must not be <c>null</c>.</param>
        /// <param name="voteCount">Number of votes.</param>
        /// <param name="voteAverage">Average vote value.</param>
        /// <returns>A new <see cref="BasicInfo" /> instance.</returns>
        public BasicInfo CreateBasicInfo(
            int thingId, string title, int voteCount, double voteAverage)
        {
            title.ThrowIfNull(nameof(title));

            return new BasicInfo(
                thingId: thingId,
                title: title,
                voteCount: voteCount,
                voteAverage: voteAverage
            );
        }

        /// <summary>
        /// Generates a <see cref="BasicInfo" /> filling any unspecified field
        /// with a deterministic value derived from the seeded random source.
        /// </summary>
        /// <param name="thingId">Optional unique identifier.</param>
        /// <param name="title">Optional title.</param>
        /// <param name="voteCount">Optional vote count.</param>
        /// <param name="voteAverage">Optional vote average.</param>
        /// <returns>A new <see cref="BasicInfo" /> instance.</returns>
        public BasicInfo GenerateBasicInfo(
            int? thingId = null,
            string? title = null,
            int? voteCount = null,
            double? voteAverage = null)
        {
            return CreateBasicInfo(
                thingId: thingId ?? GenerateThingId(),
                title: title ?? GenerateTitle(),
                voteCount: voteCount ?? GenerateVoteCount(),
                voteAverage: voteAverage ?? GenerateVoteAverage()
            );
        }

        /// <summary>
        /// Generates a deterministic <see cref="BasicInfo.ThingId" /> in the
        /// range [1, 1_000_000).
        /// </summary>
        public int GenerateThingId()
        {
            return _random.Next(1, 1_000_000);
        }

        /// <summary>
        /// Generates a unique title using a random GUID suffix. Not seeded
        /// (GUIDs are global) — use <see cref="CreateBasicInfo" /> when an
        /// exact title is needed.
        /// </summary>
        public string GenerateTitle()
        {
            return $"Title-{Guid.NewGuid():N}";
        }

        /// <summary>
        /// Generates a deterministic vote count in the range [10, 10_000).
        /// </summary>
        public int GenerateVoteCount()
        {
            return _random.Next(10, 10_000);
        }

        /// <summary>
        /// Generates a deterministic vote average in the range [0.0, 10.0]
        /// rounded to one decimal place.
        /// </summary>
        public double GenerateVoteAverage()
        {
            return Math.Round(_random.NextDouble() * 10.0, 1);
        }
    }
}

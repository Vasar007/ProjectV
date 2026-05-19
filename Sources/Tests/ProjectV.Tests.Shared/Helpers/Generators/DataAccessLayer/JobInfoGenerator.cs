using Acolyte.Assertions;
using ProjectV.Models.Internal.Jobs;
using ProjectV.Tests.Shared.Helpers.Generators.Models;

namespace ProjectV.Tests.Shared.Helpers.Generators.DataAccessLayer
{
    /// <summary>
    /// Generator for <see cref="JobInfo" /> test data. Follows the
    /// <c>Create(...)</c> / <c>Generate(...)</c> twin pattern (Decision D-34):
    /// <list type="bullet">
    ///     <item>
    ///         <description><c>Create*</c> — every argument is explicit; the
    ///         caller is responsible for the resulting <see cref="JobInfo" />
    ///         being valid.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>Generate*</c> — every argument is optional;
    ///         unspecified values come from a deterministic seeded
    ///         <see cref="Random" /> (seed 42 per Specifics §5).</description>
    ///     </item>
    /// </list>
    /// </summary>
    public sealed class JobInfoGenerator
    {
        private static readonly Random _random = new Random(Seed: 42);

        private readonly JobIdGenerator _jobIdGenerator;


        /// <summary>
        /// Initializes a new instance of the <see cref="JobInfoGenerator" />
        /// class with a default <see cref="JobIdGenerator" />.
        /// </summary>
        public JobInfoGenerator()
            : this(new JobIdGenerator())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JobInfoGenerator" />
        /// class with a caller-supplied <see cref="JobIdGenerator" /> — useful
        /// when a test needs a specific <see cref="JobId" /> series.
        /// </summary>
        /// <param name="jobIdGenerator">Generator for the <c>id</c> field.</param>
        public JobInfoGenerator(JobIdGenerator jobIdGenerator)
        {
            _jobIdGenerator = jobIdGenerator.ThrowIfNull(nameof(jobIdGenerator));
        }

        /// <summary>
        /// Creates a <see cref="JobInfo" /> with every field supplied
        /// explicitly by the caller.
        /// </summary>
        /// <param name="id">Job identifier — must be specified (non-default).</param>
        /// <param name="name">Job name — must not be null or whitespace.</param>
        /// <param name="state">Job state code.</param>
        /// <param name="result">Job result code.</param>
        /// <param name="config">Job configuration payload — must not be null or whitespace.</param>
        /// <returns>A new <see cref="JobInfo" /> instance.</returns>
        public JobInfo CreateJobInfo(
            JobId id, string name, int state, int result, string config)
        {
            name.ThrowIfNullOrWhiteSpace(nameof(name));
            config.ThrowIfNullOrWhiteSpace(nameof(config));

            return new JobInfo(
                id: id,
                name: name,
                state: state,
                result: result,
                config: config
            );
        }

        /// <summary>
        /// Generates a <see cref="JobInfo" /> filling any unspecified field
        /// with a deterministic value derived from the seeded random source.
        /// </summary>
        /// <param name="id">Optional job identifier.</param>
        /// <param name="name">Optional job name.</param>
        /// <param name="state">Optional state code.</param>
        /// <param name="result">Optional result code.</param>
        /// <param name="config">Optional configuration payload.</param>
        /// <returns>A new <see cref="JobInfo" /> instance.</returns>
        public JobInfo GenerateJobInfo(
            JobId? id = null,
            string? name = null,
            int? state = null,
            int? result = null,
            string? config = null)
        {
            return CreateJobInfo(
                id: id ?? GenerateId(),
                name: name ?? GenerateName(),
                state: state ?? GenerateState(),
                result: result ?? GenerateResult(),
                config: config ?? GenerateConfig()
            );
        }

        /// <summary>
        /// Generates a fresh <see cref="JobId" /> via the underlying
        /// <see cref="JobIdGenerator" />.
        /// </summary>
        public JobId GenerateId()
        {
            return _jobIdGenerator.GenerateJobId();
        }

        /// <summary>
        /// Generates a unique job name with a GUID-derived suffix.
        /// </summary>
        public string GenerateName()
        {
            return $"job-{Guid.NewGuid():N}";
        }

        /// <summary>
        /// Generates a deterministic job state code in the range [0, 100).
        /// </summary>
        public int GenerateState()
        {
            return _random.Next(0, 100);
        }

        /// <summary>
        /// Generates a deterministic job result code in the range [0, 100).
        /// </summary>
        public int GenerateResult()
        {
            return _random.Next(0, 100);
        }

        /// <summary>
        /// Generates a deterministic non-empty configuration payload using a
        /// GUID-derived suffix; ProjectV stores the raw XML/JSON config as a
        /// string in the <c>config</c> column.
        /// </summary>
        public string GenerateConfig()
        {
            return $"<config id=\"{Guid.NewGuid():N}\"/>";
        }
    }
}

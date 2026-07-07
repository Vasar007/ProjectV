using System;
using ProjectV.Models.Internal.Jobs;
using ProjectV.Tests.Shared.ForTests;
using ProjectV.Tests.Shared.Helpers.Generators.Models;
using Xunit;

namespace ProjectV.Models.Tests.ValueObjects
{
    /// <summary>
    /// Unit tests for the <see cref="JobId" /> value-object. All test cases
    /// are inherited from <see cref="BaseGuidWrapperTests{TId}" /> —
    /// <c>Create</c>, <c>Wrap</c>, <c>Parse</c>, <c>TryParse</c>,
    /// <c>None</c>, and <c>IsSpecified</c> coverage lives on the base
    /// class; this class only bridges the <see cref="JobId" /> static
    /// surface and <see cref="JobIdGenerator" /> through the base-class
    /// hooks.
    /// </summary>
    [Trait("Category", "Unit")]
    public sealed class JobIdTests : BaseGuidWrapperTests<JobId>
    {
        private readonly JobIdGenerator _generator;

        public JobIdTests()
        {
            _generator = JobIdGenerator.Instance;
        }

        /// <inheritdoc />
        protected override JobId None => JobId.None;

        /// <inheritdoc />
        protected override JobId Create()
        {
            return JobId.Create();
        }

        /// <inheritdoc />
        protected override JobId Wrap(Guid id)
        {
            return JobId.Wrap(id);
        }

        /// <inheritdoc />
        protected override JobId Parse(string rawId)
        {
            return JobId.Parse(rawId);
        }

        /// <inheritdoc />
        protected override bool TryParse(string? rawId, out JobId result)
        {
            return JobId.TryParse(rawId, out result);
        }

        /// <inheritdoc />
        protected override Guid GetValue(JobId id)
        {
            return id.Value;
        }

        /// <inheritdoc />
        protected override bool GetIsSpecified(JobId id)
        {
            return id.IsSpecified;
        }

        /// <inheritdoc />
        protected override JobId GenerateId()
        {
            return _generator.GenerateJobId();
        }

        /// <inheritdoc />
        protected override JobId CreateId(string rawId)
        {
            return _generator.CreateJobId(rawId);
        }

        /// <inheritdoc />
        protected override string GenerateRawId()
        {
            return _generator.GenerateRawId();
        }
    }
}

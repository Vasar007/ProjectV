using System;
using ProjectV.Models.Users;
using ProjectV.Tests.Shared.ForTests;
using ProjectV.Tests.Shared.Helpers.Generators.Models;
using Xunit;

namespace ProjectV.Models.Tests.ValueObjects
{
    /// <summary>
    /// Unit tests for the <see cref="UserId" /> value-object. All test cases
    /// are inherited from <see cref="BaseGuidWrapperTests{TId}" /> —
    /// <c>Create</c>, <c>Wrap</c>, <c>Parse</c>, <c>TryParse</c>,
    /// <c>None</c>, and <c>IsSpecified</c> coverage lives on the base
    /// class; this class only bridges the <see cref="UserId" /> static
    /// surface and <see cref="UserIdGenerator" /> through the base-class
    /// hooks.
    /// </summary>
    [Trait("Category", "Unit")]
    public sealed class UserIdTests : BaseGuidWrapperTests<UserId>
    {
        private readonly UserIdGenerator _generator;

        public UserIdTests()
        {
            _generator = UserIdGenerator.Instance;
        }

        /// <inheritdoc />
        protected override UserId None => UserId.None;

        /// <inheritdoc />
        protected override UserId Create()
        {
            return UserId.Create();
        }

        /// <inheritdoc />
        protected override UserId Wrap(Guid id)
        {
            return UserId.Wrap(id);
        }

        /// <inheritdoc />
        protected override UserId Parse(string rawId)
        {
            return UserId.Parse(rawId);
        }

        /// <inheritdoc />
        protected override bool TryParse(string? rawId, out UserId result)
        {
            return UserId.TryParse(rawId, out result);
        }

        /// <inheritdoc />
        protected override Guid GetValue(UserId id)
        {
            return id.Value;
        }

        /// <inheritdoc />
        protected override bool GetIsSpecified(UserId id)
        {
            return id.IsSpecified;
        }

        /// <inheritdoc />
        protected override UserId GenerateId()
        {
            return _generator.GenerateUserId();
        }

        /// <inheritdoc />
        protected override UserId CreateId(string rawId)
        {
            return _generator.CreateUserId(rawId);
        }

        /// <inheritdoc />
        protected override string GenerateRawId()
        {
            return _generator.GenerateRawId();
        }
    }
}

using System;
using ProjectV.Models.Exceptions;
using ProjectV.Tests.Shared.ForTests;
using Xunit;

namespace ProjectV.Models.Tests.Exceptions
{
    /// <summary>
    /// Verifies the 3-constructor convention on
    /// <see cref="CannotGetTmdbConfigException" /> by inheriting
    /// <see cref="BaseExceptionTests{TException}" />. Pre-canned
    /// <c>[Fact]</c> methods on the base class cover the default,
    /// message-only, and message + inner-exception constructors.
    /// </summary>
    [Trait("Category", "Unit")]
    public sealed class CannotGetTmdbConfigExceptionTests
        : BaseExceptionTests<CannotGetTmdbConfigException>
    {
        public CannotGetTmdbConfigExceptionTests()
        {
        }

        /// <inheritdoc />
        protected override CannotGetTmdbConfigException Create()
        {
            return new CannotGetTmdbConfigException();
        }

        /// <inheritdoc />
        protected override CannotGetTmdbConfigException Create(string message)
        {
            return new CannotGetTmdbConfigException(message);
        }

        /// <inheritdoc />
        protected override CannotGetTmdbConfigException Create(
            string message, Exception innerException)
        {
            return new CannotGetTmdbConfigException(message, innerException);
        }
    }
}

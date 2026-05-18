namespace ProjectV.Tests.Shared.ForTests
{
    /// <summary>
    /// Base class for tests that verify the standard 3-constructor convention
    /// every custom exception in ProjectV follows
    /// (<c>()</c>, <c>(string message)</c>,
    /// <c>(string message, Exception innerException)</c> — see CONVENTIONS.md).
    /// Concrete test classes implement the <see cref="Create()" /> /
    /// <see cref="Create(string)" /> /
    /// <see cref="Create(string, Exception)" /> factory hooks for their
    /// specific <typeparamref name="TException" /> type (Decision D-32).
    /// </summary>
    /// <typeparam name="TException">Exception type under test.</typeparam>
    public abstract class BaseExceptionTests<TException> : BaseTest
        where TException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="BaseExceptionTests{TException}" /> class.
        /// </summary>
        protected BaseExceptionTests()
        {
        }

        /// <summary>
        /// Creates a <typeparamref name="TException" /> via the default
        /// (parameterless) constructor.
        /// </summary>
        protected abstract TException Create();

        /// <summary>
        /// Creates a <typeparamref name="TException" /> via the
        /// message-only constructor.
        /// </summary>
        /// <param name="message">Exception message.</param>
        protected abstract TException Create(string message);

        /// <summary>
        /// Creates a <typeparamref name="TException" /> via the
        /// message + inner-exception constructor.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Wrapped exception.</param>
        protected abstract TException Create(string message, Exception innerException);

        [Fact]
        public void DefaultConstructor_CreatesException()
        {
            // Arrange. / Act.
            var ex = Create();

            // Assert.
            ex.Should().NotBeNull();
        }

        [Fact]
        public void MessageConstructor_SetsMessage()
        {
            // Arrange.
            const string message = "Test exception message.";

            // Act.
            var ex = Create(message);

            // Assert.
            ex.Message.Should().Be(message);
        }

        [Fact]
        public void InnerExceptionConstructor_SetsMessageAndInnerException()
        {
            // Arrange.
            const string message = "Test exception message.";
            var inner = new InvalidOperationException("inner");

            // Act.
            var ex = Create(message, inner);

            // Assert.
            ex.Message.Should().Be(message);
            ex.InnerException.Should().BeSameAs(inner);
        }
    }
}

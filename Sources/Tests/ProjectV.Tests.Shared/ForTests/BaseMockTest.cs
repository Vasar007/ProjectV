namespace ProjectV.Tests.Shared.ForTests
{
    /// <summary>
    /// Base class for unit tests that need NSubstitute substitutes (the
    /// project's chosen mocking library — Decision D-05). Exposes a small
    /// <see cref="CreateMock{T}" /> convenience that wraps
    /// <see cref="Substitute.For{T}()" />.
    /// </summary>
    /// <remarks>
    /// New tests should prefer the <c>Test*Builder</c> classes under
    /// <c>Helpers/Mocks/</c> (Decision D-33) over hand-rolling substitutes.
    /// </remarks>
    public abstract class BaseMockTest : BaseTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMockTest" /> class.
        /// </summary>
        protected BaseMockTest()
        {
        }

        /// <summary>
        /// Creates an <see cref="NSubstitute" /> substitute for the requested
        /// interface or virtual class.
        /// </summary>
        /// <typeparam name="T">Type to substitute. Must be a reference type.</typeparam>
        /// <returns>A configured <see cref="NSubstitute" /> proxy.</returns>
        protected static T CreateMock<T>()
            where T : class
        {
            return Substitute.For<T>();
        }
    }
}

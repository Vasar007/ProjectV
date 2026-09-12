namespace ProjectV.Tests.Shared.ForTests
{
    /// <summary>
    /// Base class for all ProjectV tests. xUnit treats every test class as its
    /// own fixture; no <c>[TestFixture]</c> attribute is needed. The constructor
    /// replaces NUnit's <c>[SetUp]</c> and <see cref="System.IDisposable" /> or
    /// <see cref="Xunit.IAsyncLifetime" /> replaces <c>[TearDown]</c>.
    /// </summary>
    public abstract class BaseTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTest" /> class.
        /// </summary>
        protected BaseTest()
        {
        }
    }
}

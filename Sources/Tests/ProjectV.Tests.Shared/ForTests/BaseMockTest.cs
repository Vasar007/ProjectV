using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace ProjectV.Tests.Shared.ForTests
{
    /// <summary>
    /// Base class for unit tests that need substitutes. Exposes an
    /// <see cref="IFixture" /> wired with the
    /// <see cref="AutoNSubstituteCustomization" /> so that
    /// <c>Fixture.Freeze&lt;T&gt;()</c> and <c>Fixture.Create&lt;T&gt;()</c>
    /// return <see cref="NSubstitute" /> proxies for any interface or
    /// virtual class without per-test boilerplate.
    /// </summary>
    /// <remarks>
    /// New tests should prefer <see cref="Fixture" /> over hand-rolling
    /// <c>Substitute.For&lt;T&gt;()</c> calls. Test-class helpers should
    /// also use <see cref="Fixture" /> rather than reaching for
    /// <c>Substitute.For</c> directly.
    /// </remarks>
    public abstract class BaseMockTest : BaseTest
    {
        /// <summary>
        /// Per-test <see cref="IFixture" /> instance. A fresh fixture is
        /// created for every test class instance (xUnit constructs a new
        /// instance per test method), so frozen substitutes do not leak
        /// across tests.
        /// </summary>
        protected IFixture Fixture { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMockTest" />
        /// class with a fresh <see cref="IFixture" />.
        /// </summary>
        protected BaseMockTest()
        {
            Fixture = CreateFixture();
        }

        /// <summary>
        /// Factory method that builds a new <see cref="IFixture" /> with the
        /// <see cref="AutoNSubstituteCustomization" /> applied. Exposed as a
        /// static so test helpers that need a fixture without inheriting
        /// from <see cref="BaseMockTest" /> can still get one configured the
        /// same way.
        /// </summary>
        /// <returns>
        /// A new <see cref="IFixture" /> with NSubstitute-backed automatic
        /// substitution.
        /// </returns>
        public static IFixture CreateFixture()
        {
            return new Fixture().Customize(new AutoNSubstituteCustomization());
        }
    }
}

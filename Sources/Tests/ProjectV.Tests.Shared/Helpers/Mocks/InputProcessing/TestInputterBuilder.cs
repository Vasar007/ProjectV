using Acolyte.Assertions;
using AutoFixture;
using ProjectV.IO.Input;

namespace ProjectV.Tests.Shared.Helpers.Mocks.InputProcessing
{
    /// <summary>
    /// Builder for <see cref="IInputter" /> test doubles backed by
    /// AutoFixture + NSubstitute. One file per interface; follow the same
    /// shape for every sibling builder in this folder.
    /// </summary>
    public sealed class TestInputterBuilder
    {
        private readonly IFixture _fixture;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TestInputterBuilder" /> class. Produces a bare
        /// NSubstitute substitute with no configured behavior — sufficient
        /// for tests that only need a placeholder <see cref="IInputter" />
        /// in the dependency graph.
        /// </summary>
        /// <param name="fixture">AutoFixture instance to create the substitute.</param>
        public TestInputterBuilder(IFixture fixture)
        {
            _fixture = fixture.ThrowIfNull(nameof(fixture));
        }

        /// <summary>
        /// Convenience factory that returns a bare-bones
        /// <see cref="IInputter" /> substitute with no configured behavior.
        /// </summary>
        /// <param name="fixture">AutoFixture instance to create the substitute.</param>
        public static IInputter CreateWithoutSetup(IFixture fixture)
        {
            fixture.ThrowIfNull(nameof(fixture));
            return new TestInputterBuilder(fixture).Build();
        }

        /// <summary>
        /// Builds the <see cref="IInputter" /> substitute. No behavior is
        /// configured; the substitute returns whatever AutoFixture / NSubstitute
        /// would by default.
        /// </summary>
        public IInputter Build()
        {
            return _fixture.Create<IInputter>();
        }
    }
}

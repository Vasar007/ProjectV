using Acolyte.Assertions;
using AutoFixture;
using ProjectV.IO.Output;

namespace ProjectV.Tests.Shared.Helpers.Mocks.OutputProcessing
{
    /// <summary>
    /// Builder for <see cref="IOutputter" /> test doubles backed by
    /// AutoFixture + NSubstitute. One file per interface; follow the same
    /// shape for every sibling builder in this folder.
    /// </summary>
    public sealed class TestOutputterBuilder
    {
        private readonly IFixture _fixture;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TestOutputterBuilder" /> class. No behavior is configured
        /// until one of the <c>With*</c> methods is called.
        /// </summary>
        /// <param name="fixture">AutoFixture instance to create the substitute.</param>
        public TestOutputterBuilder(IFixture fixture)
        {
            _fixture = fixture.ThrowIfNull(nameof(fixture));
        }

        /// <summary>
        /// Convenience factory that returns a bare-bones
        /// <see cref="IOutputter" /> substitute with no configured behavior.
        /// </summary>
        /// <param name="fixture">AutoFixture instance to create the substitute.</param>
        public static IOutputter CreateWithoutSetup(IFixture fixture)
        {
            fixture.ThrowIfNull(nameof(fixture));
            return new TestOutputterBuilder(fixture).Build();
        }

        /// <summary>
        /// Builds the <see cref="IOutputter" /> substitute. No behavior is
        /// configured; the substitute returns whatever AutoFixture / NSubstitute
        /// would by default.
        /// </summary>
        public IOutputter Build()
        {
            return _fixture.Create<IOutputter>();
        }
    }
}

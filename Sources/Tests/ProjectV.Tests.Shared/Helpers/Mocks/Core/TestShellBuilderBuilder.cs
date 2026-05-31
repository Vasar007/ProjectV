using Acolyte.Assertions;
using AutoFixture;
using ProjectV.Core;
using ProjectV.Core.ShellBuilders;

namespace ProjectV.Tests.Shared.Helpers.Mocks.Core
{
    /// <summary>
    /// Builder for <see cref="IShellBuilder" /> test doubles backed by
    /// AutoFixture + NSubstitute. One file per interface; follow the same
    /// shape for every sibling builder in this folder.
    /// </summary>
    public sealed class TestShellBuilderBuilder
    {
        private readonly IFixture _fixture;

        private Shell? _getResult;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TestShellBuilderBuilder" /> class. No behavior is
        /// configured until one of the <c>With*</c> methods is called.
        /// </summary>
        /// <param name="fixture">AutoFixture instance to create the substitute.</param>
        public TestShellBuilderBuilder(IFixture fixture)
        {
            _fixture = fixture.ThrowIfNull(nameof(fixture));
        }

        /// <summary>
        /// Convenience factory that returns a bare-bones
        /// <see cref="IShellBuilder" /> substitute with no configured behavior.
        /// </summary>
        /// <param name="fixture">AutoFixture instance to create the substitute.</param>
        public static IShellBuilder CreateWithoutSetup(IFixture fixture)
        {
            fixture.ThrowIfNull(nameof(fixture));
            return new TestShellBuilderBuilder(fixture).Build();
        }

        /// <summary>
        /// Configures the substitute so that
        /// <see cref="IShellBuilder.GetResult" /> returns the supplied
        /// <paramref name="result" />.
        /// </summary>
        /// <param name="result">
        /// <see cref="Shell" /> instance to return. Must not be <c>null</c>.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestShellBuilderBuilder WithGetResult(Shell result)
        {
            _getResult = result.ThrowIfNull(nameof(result));
            return this;
        }

        /// <summary>
        /// Builds the <see cref="IShellBuilder" /> substitute. If
        /// <see cref="WithGetResult" /> has been called, every
        /// <see cref="IShellBuilder.GetResult" /> call will return the
        /// configured <see cref="Shell" />; otherwise the substitute returns
        /// whatever AutoFixture / NSubstitute would by default.
        /// </summary>
        public IShellBuilder Build()
        {
            var substitute = _fixture.Create<IShellBuilder>();

            if (_getResult is not null)
            {
                substitute.GetResult().Returns(_getResult);
            }

            return substitute;
        }
    }
}

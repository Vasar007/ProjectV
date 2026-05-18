using Acolyte.Assertions;
using ProjectV.Appraisers;

namespace ProjectV.Tests.Shared.Helpers.Mocks.Appraisers
{
    /// <summary>
    /// Builder for real <see cref="AppraisersManager" /> instances populated
    /// with <see cref="NSubstitute" /> child <see cref="IAppraiser" /> doubles
    /// (Decision D-33).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="AppraisersManager" /> is a <c>sealed</c> concrete class
    /// without a substitution-friendly interface seam, so this builder
    /// returns a real manager populated through its public
    /// <see cref="AppraisersManager.Add(IAppraiser)" /> API — pre-wired with
    /// child <see cref="IAppraiser" /> substitutes. Use
    /// <see cref="TestAppraiserBuilder" /> to configure the children
    /// (call-shape, return values, etc.).
    /// </para>
    /// <para>
    /// Sibling to <see cref="TestAppraiserBuilder" />; same shape — one file
    /// per public type that needs a test double.
    /// </para>
    /// </remarks>
    public sealed class TestAppraisersManagerBuilder
    {
        private readonly List<IAppraiser> _appraisers = new List<IAppraiser>();
        private bool _outputResults;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TestAppraisersManagerBuilder" /> class. No appraisers
        /// are registered until one of the <c>With*</c> methods is called.
        /// </summary>
        public TestAppraisersManagerBuilder()
        {
        }

        /// <summary>
        /// Convenience factory that returns an empty
        /// <see cref="AppraisersManager" /> with no children registered and
        /// <c>outputResults</c> set to <c>false</c>.
        /// </summary>
        public static AppraisersManager CreateWithoutSetup()
        {
            return new TestAppraisersManagerBuilder().Build();
        }

        /// <summary>
        /// Sets the <c>outputResults</c> flag on the resulting
        /// <see cref="AppraisersManager" />.
        /// </summary>
        /// <param name="outputResults">
        /// Whether the manager should print appraiser results to
        /// <c>GlobalMessageHandler</c>. Defaults to <c>false</c>.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestAppraisersManagerBuilder WithOutputResults(bool outputResults)
        {
            _outputResults = outputResults;
            return this;
        }

        /// <summary>
        /// Registers an <see cref="IAppraiser" /> child to be added to the
        /// resulting <see cref="AppraisersManager" />.
        /// </summary>
        /// <param name="appraiser">
        /// Appraiser substitute to register. Must not be <c>null</c>.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestAppraisersManagerBuilder WithAppraiser(IAppraiser appraiser)
        {
            appraiser.ThrowIfNull(nameof(appraiser));

            _appraisers.Add(appraiser);
            return this;
        }

        /// <summary>
        /// Registers a batch of <see cref="IAppraiser" /> children at once.
        /// </summary>
        /// <param name="appraisers">
        /// Appraiser substitutes to register. Must not be <c>null</c>; null
        /// elements are rejected.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestAppraisersManagerBuilder WithAppraisers(
            IReadOnlyList<IAppraiser> appraisers)
        {
            appraisers.ThrowIfNull(nameof(appraisers));

            foreach (IAppraiser appraiser in appraisers)
            {
                appraiser.ThrowIfNull(nameof(appraisers));
                _appraisers.Add(appraiser);
            }

            return this;
        }

        /// <summary>
        /// Builds the <see cref="AppraisersManager" /> instance pre-populated
        /// with the registered children. The manager is a real production
        /// object — children are added via its public
        /// <see cref="AppraisersManager.Add(IAppraiser)" /> method.
        /// </summary>
        public AppraisersManager Build()
        {
            var manager = new AppraisersManager(_outputResults);
            foreach (IAppraiser appraiser in _appraisers)
            {
                manager.Add(appraiser);
            }

            return manager;
        }
    }
}

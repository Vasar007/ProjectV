using Acolyte.Assertions;
using ProjectV.IO.Output;

namespace ProjectV.Tests.Shared.Helpers.Stubs.Managers
{
    /// <summary>
    /// Builder for real <see cref="OutputManager" /> instances populated with
    /// <see cref="NSubstitute" /> child <see cref="IOutputter" /> doubles.
    /// <see cref="OutputManager" /> is <c>sealed</c> without a
    /// substitution-friendly interface seam, so this
    /// builder returns a real manager populated through its public
    /// <see cref="OutputManager.Add(IOutputter)" /> API.
    /// </summary>
    /// <remarks>
    /// The default storage name is a non-empty placeholder because the
    /// production constructor calls <c>ThrowIfNullOrWhiteSpace</c> on it.
    /// </remarks>
    public sealed class TestOutputManagerBuilder
    {
        /// <summary>
        /// Default storage name used by <see cref="CreateWithoutSetup" /> and
        /// builds that do not call <see cref="WithDefaultStorageName" />.
        /// Non-empty to satisfy the production ctor guard.
        /// </summary>
        public const string DefaultStorageName = "test-output-storage";

        private readonly List<IOutputter> _outputters = new List<IOutputter>();
        private string _defaultStorageName = DefaultStorageName;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TestOutputManagerBuilder" /> class. No outputters are
        /// registered until one of the <c>With*</c> methods is called.
        /// </summary>
        public TestOutputManagerBuilder()
        {
        }

        /// <summary>
        /// Convenience factory that returns an empty
        /// <see cref="OutputManager" /> with no child outputters registered.
        /// </summary>
        public static OutputManager CreateWithoutSetup()
        {
            return new TestOutputManagerBuilder().Build();
        }

        /// <summary>
        /// Overrides the default storage name passed to the
        /// <see cref="OutputManager" /> constructor.
        /// </summary>
        /// <param name="defaultStorageName">
        /// Storage name. Must not be <c>null</c>, empty, or whitespace.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestOutputManagerBuilder WithDefaultStorageName(string defaultStorageName)
        {
            defaultStorageName.ThrowIfNullOrWhiteSpace(nameof(defaultStorageName));

            _defaultStorageName = defaultStorageName;
            return this;
        }

        /// <summary>
        /// Registers an <see cref="IOutputter" /> child to be added to the
        /// resulting <see cref="OutputManager" />.
        /// </summary>
        /// <param name="outputter">
        /// Outputter substitute to register. Must not be <c>null</c>.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestOutputManagerBuilder WithOutputter(IOutputter outputter)
        {
            outputter.ThrowIfNull(nameof(outputter));

            _outputters.Add(outputter);
            return this;
        }

        /// <summary>
        /// Registers a batch of <see cref="IOutputter" /> children at once.
        /// </summary>
        /// <param name="outputters">
        /// Outputter substitutes to register. Must not be <c>null</c>; null
        /// elements are rejected.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestOutputManagerBuilder WithOutputters(IReadOnlyList<IOutputter> outputters)
        {
            outputters.ThrowIfNull(nameof(outputters));

            foreach (IOutputter outputter in outputters)
            {
                outputter.ThrowIfNull(nameof(outputters));
                _outputters.Add(outputter);
            }

            return this;
        }

        /// <summary>
        /// Builds the <see cref="OutputManager" /> instance pre-populated
        /// with the registered children.
        /// </summary>
        public OutputManager Build()
        {
            var manager = new OutputManager(_defaultStorageName);
            foreach (IOutputter outputter in _outputters)
            {
                manager.Add(outputter);
            }

            return manager;
        }
    }
}

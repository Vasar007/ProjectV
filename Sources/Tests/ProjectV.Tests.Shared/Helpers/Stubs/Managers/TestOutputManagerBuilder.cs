using ProjectV.IO.Output;

namespace ProjectV.Tests.Shared.Helpers.Stubs.Managers
{
    /// <summary>
    /// Builder for real (empty) <see cref="OutputManager" /> instances.
    /// <see cref="OutputManager" /> is <c>sealed</c> without a
    /// substitution-friendly interface seam, so this
    /// builder returns a real manager.
    /// </summary>
    /// <remarks>
    /// The default storage name is a non-empty placeholder because the
    /// production constructor calls <c>ThrowIfNullOrWhiteSpace</c> on it.
    /// </remarks>
    public sealed class TestOutputManagerBuilder
    {
        /// <summary>
        /// Default storage name used for every build. Non-empty to satisfy
        /// the production ctor guard.
        /// </summary>
        public const string DefaultStorageName = "test-output-storage";

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TestOutputManagerBuilder" /> class.
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
        /// Builds an empty <see cref="OutputManager" /> instance with the
        /// <see cref="DefaultStorageName" />.
        /// </summary>
        public OutputManager Build()
        {
            return new OutputManager(DefaultStorageName);
        }
    }
}

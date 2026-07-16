using ProjectV.IO.Input;

namespace ProjectV.Tests.Shared.Helpers.Stubs.Managers
{
    /// <summary>
    /// Builder for real (empty) <see cref="InputManager" /> instances.
    /// <see cref="InputManager" /> is <c>sealed</c> without a
    /// substitution-friendly interface seam, so this builder
    /// returns a real manager.
    /// </summary>
    /// <remarks>
    /// Mirrors <see cref="Stubs.Appraisers.TestAppraisersManagerBuilder" /> — one
    /// file per public manager type that needs a test double. The default
    /// storage name is a non-empty placeholder because the production
    /// constructor calls <c>ThrowIfNullOrWhiteSpace</c> on it.
    /// </remarks>
    public sealed class TestInputManagerBuilder
    {
        /// <summary>
        /// Default storage name used for every build. Non-empty to satisfy
        /// the production ctor guard.
        /// </summary>
        public const string DefaultStorageName = "test-input-storage";

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TestInputManagerBuilder" /> class.
        /// </summary>
        public TestInputManagerBuilder()
        {
        }

        /// <summary>
        /// Convenience factory that returns an empty
        /// <see cref="InputManager" /> with no child inputters registered.
        /// </summary>
        public static InputManager CreateWithoutSetup()
        {
            return new TestInputManagerBuilder().Build();
        }

        /// <summary>
        /// Builds an empty <see cref="InputManager" /> instance with the
        /// <see cref="DefaultStorageName" />.
        /// </summary>
        public InputManager Build()
        {
            return new InputManager(DefaultStorageName);
        }
    }
}

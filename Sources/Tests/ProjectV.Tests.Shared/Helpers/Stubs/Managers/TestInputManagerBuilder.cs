using Acolyte.Assertions;
using ProjectV.IO.Input;

namespace ProjectV.Tests.Shared.Helpers.Stubs.Managers
{
    /// <summary>
    /// Builder for real <see cref="InputManager" /> instances populated with
    /// <see cref="NSubstitute" /> child <see cref="IInputter" /> doubles.
    /// <see cref="InputManager" /> is <c>sealed</c> without a
    /// substitution-friendly interface seam, so this builder
    /// returns a real manager populated through its public
    /// <see cref="InputManager.Add(IInputter)" /> API.
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
        /// Default storage name used by <see cref="CreateWithoutSetup" /> and
        /// builds that do not call <see cref="WithDefaultStorageName" />.
        /// Non-empty to satisfy the production ctor guard.
        /// </summary>
        public const string DefaultStorageName = "test-input-storage";

        private readonly List<IInputter> _inputters = new List<IInputter>();
        private string _defaultStorageName = DefaultStorageName;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TestInputManagerBuilder" /> class. No inputters are
        /// registered until one of the <c>With*</c> methods is called.
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
        /// Overrides the default storage name passed to the
        /// <see cref="InputManager" /> constructor.
        /// </summary>
        /// <param name="defaultStorageName">
        /// Storage name. Must not be <c>null</c>, empty, or whitespace.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestInputManagerBuilder WithDefaultStorageName(string defaultStorageName)
        {
            defaultStorageName.ThrowIfNullOrWhiteSpace(nameof(defaultStorageName));

            _defaultStorageName = defaultStorageName;
            return this;
        }

        /// <summary>
        /// Registers an <see cref="IInputter" /> child to be added to the
        /// resulting <see cref="InputManager" />.
        /// </summary>
        /// <param name="inputter">
        /// Inputter substitute to register. Must not be <c>null</c>.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestInputManagerBuilder WithInputter(IInputter inputter)
        {
            inputter.ThrowIfNull(nameof(inputter));

            _inputters.Add(inputter);
            return this;
        }

        /// <summary>
        /// Registers a batch of <see cref="IInputter" /> children at once.
        /// </summary>
        /// <param name="inputters">
        /// Inputter substitutes to register. Must not be <c>null</c>; null
        /// elements are rejected.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestInputManagerBuilder WithInputters(IReadOnlyList<IInputter> inputters)
        {
            inputters.ThrowIfNull(nameof(inputters));

            foreach (IInputter inputter in inputters)
            {
                inputter.ThrowIfNull(nameof(inputters));
                _inputters.Add(inputter);
            }

            return this;
        }

        /// <summary>
        /// Builds the <see cref="InputManager" /> instance pre-populated with
        /// the registered children.
        /// </summary>
        public InputManager Build()
        {
            var manager = new InputManager(_defaultStorageName);
            foreach (IInputter inputter in _inputters)
            {
                manager.Add(inputter);
            }

            return manager;
        }
    }
}

using Acolyte.Assertions;

namespace ThingAppraiser.Core.ShellBuilders
{
    /// <summary>
    /// Builder director which controls of <see cref="ShellAsync" /> creating process with the help
    /// of specified builder.
    /// </summary>
    public sealed class ShellAsyncBuilderDirector
    {
        /// <summary>
        /// Builder which create step by step <see cref="ShellAsync" /> class.
        /// </summary>
        private IShellAsyncBuilder _shellBuilder;


        /// <summary>
        /// Initializes director with passed builder.
        /// </summary>
        /// <param name="shellBuilder">Instance of builder.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="shellBuilder" /> is <c>null</c>.
        /// </exception>
        public ShellAsyncBuilderDirector(IShellAsyncBuilder shellBuilder)
        {
            _shellBuilder = shellBuilder.ThrowIfNull(nameof(shellBuilder));
        }

        /// <summary>
        /// Changes builder instance.
        /// </summary>
        /// <param name="newBuilder">New builder to set.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="newBuilder" /> is <c>null</c>.
        /// </exception>
        public void ChangeShellBuilder(IShellAsyncBuilder newBuilder)
        {
            _shellBuilder = newBuilder.ThrowIfNull(nameof(newBuilder));
        }

        /// <summary>
        /// Executes building process and gets result.
        /// </summary>
        /// <returns>Fully initialized <see cref="ShellAsync" /> class.</returns>
        public ShellAsync MakeShell()
        {
            _shellBuilder.Reset();

            _shellBuilder.BuildMessageHandler();
            _shellBuilder.BuildInputManager();
            _shellBuilder.BuildCrawlersManager();
            _shellBuilder.BuildAppraisersManager();
            _shellBuilder.BuildOutputManager();

            return _shellBuilder.GetResult();
        }
    }
}

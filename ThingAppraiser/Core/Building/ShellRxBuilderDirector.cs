namespace ThingAppraiser.Core.Building
{
    /// <summary>
    /// Builder director which controls of <see cref="ShellRx" /> creating process with the help of
    /// specified builder.
    /// </summary>
    public class ShellRxBuilderDirector
    {
        /// <summary>
        /// Builder which create step by step <see cref="ShellRx" /> class.
        /// </summary>
        private IShellRxBuilder _shellBuilder;


        /// <summary>
        /// Initializes director with passed builder.
        /// </summary>
        /// <param name="shellBuilder">Instance of builder.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="shellBuilder" /> is <c>null</c>.
        /// </exception>
        public ShellRxBuilderDirector(IShellRxBuilder shellBuilder)
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
        public void ChangeShellBuilder(IShellRxBuilder newBuilder)
        {
            _shellBuilder = newBuilder.ThrowIfNull(nameof(newBuilder));
        }

        /// <summary>
        /// Executes building process and gets result.
        /// </summary>
        /// <returns>Fully initialized <see cref="ShellRx" /> class.</returns>
        public ShellRx MakeShell()
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

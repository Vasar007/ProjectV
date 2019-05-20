namespace ThingAppraiser.Core.Building
{
    /// <summary>
    /// Builder director which controls of <see cref="Shell" /> creating process with the help of
    /// specified builder.
    /// </summary>
    public class ShellBuilderDirector
    {
        /// <summary>
        /// Builder which create step by step <see cref="Shell" /> class.
        /// </summary>
        private IShellBuilder _shellBuilder;


        /// <summary>
        /// Initializes director with passed builder.
        /// </summary>
        /// <param name="shellBuilder">Instance of builder.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="shellBuilder" /> is <c>null</c>.
        /// </exception>
        public ShellBuilderDirector(IShellBuilder shellBuilder)
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
        public void ChangeShellBuilder(IShellBuilder newBuilder)
        {
            _shellBuilder = newBuilder.ThrowIfNull(nameof(newBuilder));
        }

        /// <summary>
        /// Executes building process and gets result.
        /// </summary>
        /// <returns>Fully initialized <see cref="Shell" /> class.</returns>
        public Shell MakeShell()
        {
            _shellBuilder.Reset();

            _shellBuilder.BuildMessageHandler();
            _shellBuilder.BuildInputManager();
            _shellBuilder.BuildCrawlersManager();
            _shellBuilder.BuildAppraisersManager();
            _shellBuilder.BuildOutputManager();
            _shellBuilder.BuildDataBaseManager();

            return _shellBuilder.GetResult();
        }
    }
}

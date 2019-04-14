namespace ThingAppraiser.Core.Building
{
    /// <summary>
    /// Builder director which controls of <see cref="CShell" /> creating process with the help of \
    /// specified builder.
    /// </summary>
    public class CShellBuilderDirector
    {
        /// <summary>
        /// Builder which create step by step <see cref="CShell" /> class.
        /// </summary>
        private IShellBuilder _shellBuilder;


        /// <summary>
        /// Initializes director with passed builder.
        /// </summary>
        /// <param name="shellBuilder">Instance of builder.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="shellBuilder">shellBuilder</paramref> is <c>null</c>.
        /// </exception>
        public CShellBuilderDirector(IShellBuilder shellBuilder)
        {
            _shellBuilder = shellBuilder.ThrowIfNull(nameof(shellBuilder));
        }

        /// <summary>
        /// Changes builder instance.
        /// </summary>
        /// <param name="newBuilder">New builder to set.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="newBuilder">newBuilder</paramref> is <c>null</c>.
        /// </exception>
        public void ChangeShellBuilder(IShellBuilder newBuilder)
        {
            _shellBuilder = newBuilder.ThrowIfNull(nameof(newBuilder));
        }

        /// <summary>
        /// Executes building process and gets result.
        /// </summary>
        /// <returns>Fully initialized <see cref="CShell" /> class.</returns>
        public CShell MakeShell()
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

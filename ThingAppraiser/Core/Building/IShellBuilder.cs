namespace ThingAppraiser.Core.Building
{
    /// <summary>
    /// Creates interface to build <see cref="CShell" /> class step by step.
    /// </summary>
    public interface IShellBuilder
    {
        /// <summary>
        /// Resets build status and allows to start build process again.
        /// </summary>
        void Reset();

        /// <summary>
        /// Initializes message handler which is used by manager classes to interact with user.
        /// </summary>
        void BuildMessageHandler();

        /// <summary>
        /// Creates input manager with inputters which can process different input sources.
        /// </summary>
        void BuildInputManager();

        /// <summary>
        /// Creates crawlers manager with crawlers which can send requests to online services and
        /// extract results from their responses.
        /// </summary>
        void BuildCrawlersManager();

        /// <summary>
        /// Creates appraisers manager with appraisers which can process raw data and transform it
        /// to specified format.
        /// </summary>
        void BuildAppraisersManager();

        /// <summary>
        /// Creates output manager with inputters which can process different output streams. 
        /// </summary>
        void BuildOutputManager();

        /// <summary>
        /// Collects all managers and initializes <see cref="CShell" /> instance.
        /// </summary>
        /// <returns>Fully initialized instance.</returns>
        CShell GetResult();
    }
}

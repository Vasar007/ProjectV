using ThingAppraiser.Data.Crawlers;

namespace ThingAppraiser.Crawlers
{
    /// <summary>
    /// Provides global thread-safe access to TMDB service configuration.
    /// </summary>
    public static class TmdbServiceConfiguration
    {
        /// <summary>
        /// Object for lock statement.
        /// </summary>
        private readonly static object _syncRoot = new object();

        /// <summary>
        /// Stores service configuration.
        /// </summary>
        public static TmdbServiceConfigurationInfo Configuration { get; private set; }


        /// <summary>
        /// Checks if configuration was initilized before.
        /// </summary>
        /// <returns></returns>
        public static bool HasValue()
        {
            return !(Configuration is null);
        }

        /// <summary>
        /// Updates configuration if it is <c>null</c>.
        /// </summary>
        /// <param name="newConfiguration">New configuration to set.</param>
        public static void SetServiceConfigurationIfNeed(
            TmdbServiceConfigurationInfo newConfiguration)
        {
            if (Configuration is null)
            {
                lock (_syncRoot)
                {
                    if (Configuration is null)
                    {
                        Configuration = newConfiguration;
                    }
                }
            }
        }
    }
}

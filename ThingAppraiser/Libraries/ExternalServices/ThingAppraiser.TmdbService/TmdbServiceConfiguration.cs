using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.TmdbService
{
    /// <summary>
    /// Provides global thread-safe access to TMDb service configuration.
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
        /// <returns><c>true</c> if configuration was initilized, <c>false</c> otherwise.</returns>
        public static bool HasValue => !(Configuration is null);

        /// <summary>
        /// Updates configuration if it is <c>null</c>.
        /// </summary>
        /// <param name="newConfiguration">New configuration to set.</param>
        /// <returns><c>true</c> if value was set, <c>false</c> otherwise.</returns>
        public static bool SetServiceConfigurationIfNeed(
            TmdbServiceConfigurationInfo newConfiguration)
        {
            if (Configuration is null)
            {
                lock (_syncRoot)
                {
                    if (Configuration is null)
                    {
                        Configuration = newConfiguration;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Changes configuration of TMDb service.
        /// </summary>
        /// <param name="newConfiguration">New configuration to set.</param>
        public static void SetServiceConfiguration(
            TmdbServiceConfigurationInfo newConfiguration)
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

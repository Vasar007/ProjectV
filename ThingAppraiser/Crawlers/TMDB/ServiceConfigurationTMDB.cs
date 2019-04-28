using System;

namespace ThingAppraiser.Crawlers
{
    /// <summary>
    /// Provides global thread-safe access to TMDB service configuration.
    /// </summary>
    public static class SServiceConfigurationTMDB
    {
        /// <summary>
        /// Synchronization object for lock statement.
        /// </summary>
        private static readonly Object s_lockObject = new Object();

        /// <summary>
        /// Value for service configuration property.
        /// </summary>
        private static CServiceConfigurationInfoTMDB s_serviceConfigurationInfoTMDB;

        /// <summary>
        /// Stores service configuration.
        /// </summary>
        public static CServiceConfigurationInfoTMDB Configuration
        {
            get
            {
                lock (s_lockObject)
                {
                    return s_serviceConfigurationInfoTMDB;
                }
            }
            set
            {
                lock (s_lockObject)
                {
                    s_serviceConfigurationInfoTMDB = value;
                }
            }
        }
    }
}

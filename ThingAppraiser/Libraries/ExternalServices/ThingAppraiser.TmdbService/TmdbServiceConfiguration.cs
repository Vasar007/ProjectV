using ThingAppraiser.Extensions;
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
        /// Back field for correspond property that contains configuration of TMDb service.
        /// </summary>
        public static TmdbServiceConfigurationInfo? _configuration;

        /// <summary>
        /// Stores service configuration.
        /// </summary>
        public static TmdbServiceConfigurationInfo Configuration
        {
#pragma warning disable CS8603 // Possible null reference return.
            get => _configuration.ThrowIfNull(nameof(_configuration));
#pragma warning restore CS8603 // Possible null reference return.
            private set => _configuration = value.ThrowIfNull(nameof(value));
        }


        /// <summary>
        /// Checks if configuration was initilized before.
        /// </summary>
        /// <returns><c>true</c> if configuration was initilized, <c>false</c> otherwise.</returns>
        public static bool HasValue => !(_configuration is null);

        /// <summary>
        /// Updates configuration if it is <c>null</c>.
        /// </summary>
        /// <param name="newConfiguration">New configuration to set.</param>
        /// <returns><c>true</c> if value was set, <c>false</c> otherwise.</returns>
        public static bool SetServiceConfigurationIfNeed(
            TmdbServiceConfigurationInfo newConfiguration)
        {
            if (_configuration is null)
            {
                lock (_syncRoot)
                {
                    if (_configuration is null)
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
                Configuration = newConfiguration;
            }
        }
    }
}

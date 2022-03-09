﻿using Acolyte.Assertions;
using ProjectV.Models.Internal;

namespace ProjectV.TmdbService
{
    /// <summary>
    /// Provides global thread-safe access to TMDb service configuration.
    /// </summary>
    public static class TmdbServiceConfiguration
    {
        /// <summary>
        /// Object for lock statement.
        /// </summary>
        private readonly static object _syncRoot = new();

        /// <summary>
        /// Back field for correspond property that contains configuration of TMDb service.
        /// </summary>
        private static TmdbServiceConfigurationInfo? _configuration;

        /// <summary>
        /// Stores service configuration.
        /// </summary>
        public static TmdbServiceConfigurationInfo Configuration
        {
            get => _configuration.ThrowIfNull(nameof(_configuration));
            private set => _configuration = value.ThrowIfNull(nameof(value));
        }


        /// <summary>
        /// Checks if configuration was initialized before.
        /// </summary>
        /// <returns><c>true</c> if configuration was initialized, <c>false</c> otherwise.</returns>
        public static bool HasValue => _configuration is not null;

        /// <summary>
        /// Updates configuration if it is <c>null</c>.
        /// </summary>
        /// <param name="newConfiguration">New configuration to set.</param>
        /// <returns><c>true</c> if value was set, <c>false</c> otherwise.</returns>
        public static bool SetServiceConfigurationOnce(
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
        public static void SetServiceConfigurationAnyway(
            TmdbServiceConfigurationInfo newConfiguration)
        {
            lock (_syncRoot)
            {
                Configuration = newConfiguration;
            }
        }
    }
}

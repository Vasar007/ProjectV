using System;
using System.Collections.Concurrent;
using Acolyte.Assertions;
using ThingAppraiser.Logging;
using ThingAppraiser.SteamService.Models;

namespace ThingAppraiser.SteamService
{
    public static class SteamAppsStorage
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(SteamAppsStorage));

        /// <summary>
        /// Synchronization object for lock statement.
        /// </summary>
        private static readonly object _syncRoot = new object();

        private static readonly ConcurrentDictionary<string, int> _steamAppsList =
            new ConcurrentDictionary<string, int>();
        
        public static bool IsEmpty => _steamAppsList.IsEmpty;


        public static bool AddValue(SteamBriefInfo steamApp)
        {
            if (!_steamAppsList.ContainsKey(steamApp.Name))
            {
                return _steamAppsList.TryAdd(steamApp.Name, steamApp.AppId);
            }
            return false;
        }

        public static void FillStorage(SteamBriefInfoContainer steamApps)
        {
            lock (_syncRoot)
            {
                ClearStorage();
                foreach (SteamBriefInfo steamApp in steamApps.Results)
                {
                    AddValue(steamApp);
                }
            }
        }

        public static void ClearStorage()
        {
            _steamAppsList.Clear();
        }

        public static int GetAppIdByName(string name)
        {
            name.ThrowIfNullOrWhiteSpace(nameof(name));

            if (!_steamAppsList.TryGetValue(name, out int appId))
            {
                throw new ArgumentException($"Couldn't find Steam App with name \"{name}\".",
                                            nameof(name));
            }
            return appId;
        }

        public static int? TryGetAppIdByName(string name)
        {
            name.ThrowIfNullOrWhiteSpace(nameof(name));

            try
            {
                return GetAppIdByName(name);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, $"Cannot get app ID by name \"{name}\".");

                return null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using Acolyte.Assertions;
using ProjectV.Logging;
using ProjectV.SteamService.Models;

namespace ProjectV.SteamService.Storages
{
    public sealed class SteamAppsStorage
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(SteamAppsStorage));

        /// <summary>
        /// Synchronization object for lock statement.
        /// </summary>
        private static readonly object _syncRoot = new();

        /// <summary>
        /// Storage collection. The main reason to use common dictionary over concurrent is method
        /// "FillStorage" should be executed as single call.
        /// No one should interrupt filling storage.
        /// </summary>
        private readonly Dictionary<string, int> _steamAppsList;

        public bool IsEmpty
        {
            get
            {
                lock (_syncRoot)
                {
                    return _steamAppsList.Count == 0;
                }
            }
        }

        public SteamAppsStorage()
        {
            _steamAppsList = new Dictionary<string, int>();
        }

        public string ConverToKey(string name)
        {
            name.ThrowIfNullOrWhiteSpace(nameof(name));

            return name.ToLowerInvariant();
        }

        public bool AddValue(SteamBriefInfo steamApp)
        {
            lock (_syncRoot)
            {
                string key = ConverToKey(steamApp.Name);
                return _steamAppsList.TryAdd(key, steamApp.AppId);
            }
        }

        public void FillStorage(SteamBriefInfoContainer steamApps)
        {
            lock (_syncRoot)
            {
                ClearStorage();
                foreach (SteamBriefInfo steamApp in steamApps.Results)
                {
                    if (string.IsNullOrWhiteSpace(steamApp.Name))
                    {
                        _logger.Warn(
                            $"Failed to add steam app with ID {steamApp.AppId.ToString()}: " +
                            $"app has empty name."
                        );
                        continue;
                    }

                    AddValue(steamApp);
                }
            }
        }

        public void ClearStorage()
        {
            lock (_syncRoot)
            {
                _steamAppsList.Clear();
            }
        }

        public int? FindAppIdByName(string name)
        {
            name.ThrowIfNullOrWhiteSpace(nameof(name));

            lock (_syncRoot)
            {
                string key = ConverToKey(name);
                if (!_steamAppsList.TryGetValue(key, out int appId))
                {
                    _logger.Warn($"Couldn't find Steam App with name \"{name}\".");
                    return null;
                }

                return appId;
            }
        }

        public int GetAppIdByName(string name)
        {
            name.ThrowIfNullOrWhiteSpace(nameof(name));

            int? appId = FindAppIdByName(name);

            if (!appId.HasValue)
            {
                throw new ArgumentException($"Couldn't find Steam App with name \"{name}\".",
                                            nameof(name));
            }

            return appId.Value;
        }

        public int? TryGetAppIdByName(string name)
        {
            name.ThrowIfNullOrWhiteSpace(nameof(name));

            try
            {
                return FindAppIdByName(name);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, $"Cannot get app ID by name \"{name}\".");

                return null;
            }
        }
    }
}

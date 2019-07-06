using System;
using System.Collections.Concurrent;
using SteamWebApiLib.Models.BriefInfo;

namespace ThingAppraiser.Crawlers
{
    public static class SteamAppsStorage
    {
        private static ConcurrentDictionary<string, int> _steamAppsList =
            new ConcurrentDictionary<string, int>();

        public static bool IsEmpty => _steamAppsList.IsEmpty;


        public static bool AddValue(SteamAppBriefInfo steamApp)
        {
            if (!_steamAppsList.ContainsKey(steamApp.Name))
            {
                return _steamAppsList.TryAdd(steamApp.Name, steamApp.AppId);
            }
            return false;
        }

        public static void FillStorage(SteamAppBriefInfoList steamApps)
        {
            _steamAppsList = new ConcurrentDictionary<string, int>(1, steamApps.Apps.Length);
            foreach (SteamAppBriefInfo steamApp in steamApps.Apps)
            {
                AddValue(steamApp);
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
                throw new ArgumentException($"Couldn't find Steam App with name {name}",
                                            nameof(name));
            }
            return appId;
        }
    }
}

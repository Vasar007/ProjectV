namespace ProjectV.SteamService.Storages
{
    public static class GlobalSteamAppsStorage
    {
        /// <summary>
        /// Synchronization object for lock statement.
        /// </summary>
        private static readonly object _syncRoot = new();

        private static SteamAppsStorage? _storage;
        public static SteamAppsStorage Instance
        {
            get
            {
                if (_storage is null)
                {
                    lock (_syncRoot)
                    {
                        if (_storage is null)
                        {
                            _storage = new SteamAppsStorage();
                        }
                    }
                }

                return _storage;
            }
        }
    }
}

using System;

namespace ThingAppraiser.DAL
{
    public class CDataStorageSettings
    {
        public String DBConnectionString { get; }

        public CDataStorageSettings(String dbConnectionString)
        {
            DBConnectionString = dbConnectionString.ThrowIfNullOrEmpty(nameof(dbConnectionString));
        }
    }
}

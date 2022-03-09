using Acolyte.Assertions;
using ProjectV.Configuration;

namespace ProjectV.DataAccessLayer
{
    public sealed class DatabaseOptions : IOptions
    {
        public string ConnectionString { get; set; }

        public bool CanUseDatabase { get; set; }


        public DatabaseOptions()
        {
            ConnectionString = string.Empty;
            CanUseDatabase = false;
        }

        public DatabaseOptions(
            string dbConnectionString,
            bool canUseDatabase)
        {
            ConnectionString = dbConnectionString.ThrowIfNullOrEmpty(nameof(dbConnectionString));
            CanUseDatabase = canUseDatabase;
        }
    }
}

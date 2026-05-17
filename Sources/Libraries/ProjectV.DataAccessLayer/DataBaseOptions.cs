using Acolyte.Assertions;
using ProjectV.Configuration;

namespace ProjectV.DataAccessLayer
{
    public sealed class DatabaseOptions : IOptions
    {
        public string ConnectionString { get; init; }

        public bool CanUseDatabase { get; init; }


        public DatabaseOptions()
        {
            ConnectionString = string.Empty;
            CanUseDatabase = false;
        }

        public DatabaseOptions(
            string dbConnectionString,
            bool canUseDatabase)
        {
            ConnectionString = dbConnectionString.ThrowIfNullOrWhiteSpace(nameof(dbConnectionString));
            CanUseDatabase = canUseDatabase;
        }

        #region IOptions Implementation

        public void Validate()
        {
            if (CanUseDatabase)
            {
                ConnectionString.ThrowIfNullOrWhiteSpace(nameof(ConnectionString));
            }
        }

        #endregion
    }
}

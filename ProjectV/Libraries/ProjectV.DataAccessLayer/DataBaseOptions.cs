using Acolyte.Assertions;
using ProjectV.Configuration;

namespace ProjectV.DataAccessLayer
{
    public sealed class DatabaseOptions : IOptions
    {
        public string ConnectionString { get; set; }


        public DatabaseOptions()
        {
            ConnectionString = string.Empty;
        }

        public DatabaseOptions(string dbConnectionString)
        {
            ConnectionString = dbConnectionString.ThrowIfNullOrEmpty(nameof(dbConnectionString));
        }
    }
}

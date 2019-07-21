namespace ThingAppraiser.DAL
{
    public class DataStorageSettings
    {
        public string DbConnectionString { get; }


        public DataStorageSettings(string dbConnectionString)
        {
            DbConnectionString = dbConnectionString.ThrowIfNullOrEmpty(nameof(dbConnectionString));
        }
    }
}

using System.Data.SqlClient;
using ThingAppraiser.DAL.Mappers;

namespace ThingAppraiser.DAL.Repositories
{
    public class DataProcessor : IDataProcessor
    {
        private readonly DataStorageSettings _dbSettings;


        public DataProcessor(DataStorageSettings dbSettings)
        {
            _dbSettings = dbSettings.ThrowIfNull(nameof(dbSettings));
        }

        #region IDataProcessor Implementation

        public T GetMinimum<T>(string columnName, string tableName)
        {
            string sqlStatement = $"SELECT MIN({columnName}) FROM {tableName}";

            using (var dbHelper = new DbHelperScope(_dbSettings))
            using (var query = new SqlCommand(sqlStatement))
            {
                return dbHelper.GetScalar<T>(query);
            }
        }

        public T GetMaximum<T>(string columnName, string tableName)
        {
            string sqlStatement = $"SELECT MAX({columnName}) FROM {tableName}";

            using (var dbHelper = new DbHelperScope(_dbSettings))
            using (var query = new SqlCommand(sqlStatement))
            {
                return dbHelper.GetScalar<T>(query);
            }
        }

        public (T, T) GetMinMax<T>(string columnName, string tableName)
        {
            string sqlStatement = $"SELECT MIN({columnName}), MAX({columnName}) FROM {tableName}";

            using (var dbHelper = new DbHelperScope(_dbSettings))
            using (var query = new SqlCommand(sqlStatement))
            {
                return dbHelper.GetData(new TwoValuesMapper<T>(), query)[0];
            }
        }

        #endregion

        //TODO: install .NET Standard 2.1 (now we have only preview) and add command sanitizing.
        /*
        private IDbCommand SanitizeCommand(DbHelper dbHelper, string columnName, string tableName)
        {
            DbConnection connection = dbHelper.GetDbConnection();

            DbProviderFactory factory = DbProviderFactories.GetFactory(connection);

            // Sanitize the table name
            DbCommandBuilder commandBuilder = factory.CreateCommandBuilder();

            string sanitizedTableName = commandBuilder.QuoteIdentifier(tableName);

            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM " + sanitizedTableName;
            return command;
        }
        */
    }
}

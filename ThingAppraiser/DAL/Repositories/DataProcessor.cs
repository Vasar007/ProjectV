using System;
using System.Data.SqlClient;
using ThingAppraiser.DAL.Mappers;

namespace ThingAppraiser.DAL.Repositories
{
    public class CDataProcessor : IDataProcessor
    {
        private readonly CDataStorageSettings _dbSettings;


        public CDataProcessor(CDataStorageSettings dbSettings)
        {
            _dbSettings = dbSettings.ThrowIfNull(nameof(dbSettings));
        }

        #region IDataProcessor Implementation

        public T GetMinimum<T>(String columnName, String tableName)
        {
            String sqlStatement = $"SELECT MIN({columnName}) " +
                                  $"FROM {tableName}";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var query = new SqlCommand(sqlStatement))
            {
                return dbHelper.GetScalar<T>(query);
            }
        }

        public T GetMaximum<T>(String columnName, String tableName)
        {
            String sqlStatement = $"SELECT MAX({columnName}) " +
                                  $"FROM {tableName}";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var query = new SqlCommand(sqlStatement))
            {
                return dbHelper.GetScalar<T>(query);
            }
        }

        public (T, T) GetMinMax<T>(String columnName, String tableName)
        {
            String sqlStatement = $"SELECT MIN({columnName}), MAX({columnName}) " +
                                  $"FROM {tableName}";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var query = new SqlCommand(sqlStatement))
            {
                return dbHelper.GetData(new CTwoValuesMapper<T>(), query)[0];
            }
        }

        #endregion

        //TODO: install .NET Standard 2.1 (now we have only preview) and add command sanitizing.
        /*
        private IDbCommand SanitizeCommand(CDBHelper dbHelper, String columnName, String tableName)
        {
            DbConnection connection = dbHelper.GetDBConnection();

            DbProviderFactory factory = DbProviderFactories.GetFactory(connection);

            // Sanitize the table name
            DbCommandBuilder commandBuilder = factory.CreateCommandBuilder();

            String sanitizedTableName = commandBuilder.QuoteIdentifier(tableName);

            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM " + sanitizedTableName;
            return command;
        }
        */
    }
}

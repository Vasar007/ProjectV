using System;
using System.Data;
using System.Data.Common;
using ThingAppraiser.DAL.DataBaseProviders;
using ThingAppraiser.DAL.Mappers;

namespace ThingAppraiser.DAL.Repositories
{
    public sealed class DataProcessor : IDataProcessor
    {
        private readonly DataStorageSettings _dbSettings;


        public DataProcessor(DataStorageSettings dbSettings)
        {
            _dbSettings = dbSettings.ThrowIfNull(nameof(dbSettings));
        }

        #region IDataProcessor Implementation

        public T GetMinimum<T>(string columnName, string tableName)
        {
            using var dbHelper = new DbHelperScope(_dbSettings);
            using var query = SanitizeCommand(dbHelper, columnName, tableName,
                                               SelectQueryType.Min);

            return dbHelper.GetScalar<T>(query);
        }

        public T GetMaximum<T>(string columnName, string tableName)
        {
            using var dbHelper = new DbHelperScope(_dbSettings);
            using var query = SanitizeCommand(dbHelper, columnName, tableName,
                                              SelectQueryType.Max);

            return dbHelper.GetScalar<T>(query);
        }

        public (T, T) GetMinMax<T>(string columnName, string tableName)
        {
            using var dbHelper = new DbHelperScope(_dbSettings);
            using var query = SanitizeCommand(dbHelper, columnName, tableName,
                                              SelectQueryType.MinMax);

            return dbHelper.GetData(new TwoValuesMapper<T>(), query)[0];
        }

        #endregion

        private IDbCommand SanitizeCommand(DbHelperScope dbHelper, string columnName,
            string tableName, SelectQueryType commandType)
        {
            DbConnection connection = dbHelper.GetDbConnection();

            DbProviderFactory factory = DbProviderFactories.GetFactory(connection);

            // Sanitize the table name.
            using DbCommandBuilder commandBuilder = factory.CreateCommandBuilder();

            string sanitizedColumnName = commandBuilder.QuoteIdentifier(columnName);
            string sanitizedTableName = commandBuilder.QuoteIdentifier(tableName);

            string commandText = commandType switch
            {
                SelectQueryType.Min =>
                    $"SELECT MIN({sanitizedColumnName}) " +
                    $"FROM {sanitizedTableName}",

                SelectQueryType.Max =>
                    $"SELECT MAX({sanitizedColumnName}) " +
                    $"FROM {sanitizedTableName}",
               
                SelectQueryType.MinMax =>
                    $"SELECT MIN({sanitizedColumnName}), MAX({sanitizedColumnName}) " +
                    $"FROM {sanitizedTableName}",

                _ => throw new ArgumentOutOfRangeException(nameof(commandType), commandType,
                                                           "Unknown command type.")
            };

            IDbCommand command = connection.CreateCommand();
            command.CommandText = commandText;
            return command;
        }
    }
}

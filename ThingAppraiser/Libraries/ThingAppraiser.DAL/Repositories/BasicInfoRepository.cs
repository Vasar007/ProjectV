using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ThingAppraiser.DAL.DataBaseProviders;
using ThingAppraiser.DAL.Mappers;
using ThingAppraiser.DAL.Properties;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.DAL.Repositories
{
    public sealed class BasicInfoRepository : IDataRepository, IRepository<BasicInfo, int>, 
        IRepositoryBase, ITagable, ITypeId
    {
        private readonly DataBaseOptions _dbSettings;

        private readonly DataProcessor _dataProcessor;

        public IReadOnlyList<string> Columns { get; } = new List<string>
        {
            "thing_id", "title", "vote_count", "vote_average"
        };

        public string TableName { get; } = "[dbo].[common]";

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(BasicInfoRepository);

        #endregion

        #region ITypeId Implementation

        /// <inheritdoc />
        public Type TypeId { get; } = typeof(BasicInfo);

        #endregion


        public BasicInfoRepository(DataBaseOptions dbSettings)
        {
            _dbSettings = dbSettings.ThrowIfNull(nameof(dbSettings));
            _dataProcessor = new DataProcessor(_dbSettings);
        }

        #region IRepository Implementation

        public bool Contains(int thingId)
        {
            string sqlStatement = SQLStatementsForCommon.CountItemsById;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var query = new SqlCommand(sqlStatement);

            query.Parameters.AddWithValue("@thing_id", thingId);

            return dbHelper.GetScalar<int>(query) > 0;
        }

        public void InsertItem(BasicInfo item)
        {
            using var dbHelper = new DbHelperScope(_dbSettings);

            InsertItem(item, dbHelper);
            dbHelper.Commit();
        }

        public BasicInfo GetItemById(int thingId)
        {
            string sqlStatement = SQLStatementsForCommon.SelectItemById;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var query = new SqlCommand(sqlStatement);

            query.Parameters.AddWithValue("@thing_id", thingId);

            return dbHelper.GetItem(new BasicInfoMapper(), query);
        }

        public IReadOnlyList<BasicInfo> GetAllData()
        {
            string sqlStatement = SQLStatementsForCommon.SelectAllItems;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var query = new SqlCommand(sqlStatement);

            return dbHelper.GetData(new BasicInfoMapper(), query);
        }

        public void UpdateItem(BasicInfo item)
        {
            using var dbHelper = new DbHelperScope(_dbSettings);

            UpdateItem(item, dbHelper);
            dbHelper.Commit();
        }

        public void DeleteItemById(int thingId)
        {
            string sqlStatement = SQLStatementsForCommon.DeleteItemById;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var command = new SqlCommand(sqlStatement);

            command.Parameters.AddWithValue("@thing_id", thingId);

            dbHelper.ExecuteCommand(command);
            dbHelper.Commit();
        }

        public void DeleteAllData()
        {
            string sqlStatement = SQLStatementsForCommon.DeleteAllItems;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var command = new SqlCommand(sqlStatement);

            dbHelper.ExecuteCommand(command);
            dbHelper.Commit();
        }

        #endregion

        #region IDataRepository Implementation

        public T GetMinimum<T>(string columnName) 
            where T : struct
        {
            if (!Columns.Contains(columnName)) return default;
            return _dataProcessor.GetMinimum<T>(columnName, TableName);
        }

        public T GetMaximum<T>(string columnName) 
            where T : struct
        {
            if (!Columns.Contains(columnName)) return default;
            return _dataProcessor.GetMaximum<T>(columnName, TableName);
        }

        public (T, T) GetMinMax<T>(string columnName) 
            where T : struct
        {
            if (!Columns.Contains(columnName)) return (default, default);
            return _dataProcessor.GetMinMax<T>(columnName, TableName);
        }

        #endregion

        public void InsertItem(BasicInfo item, DbHelperScope dbHelper)
        {
            string sqlStatement = SQLStatementsForCommon.InsertItem;

            using var command = new SqlCommand(sqlStatement);

            command.Parameters.AddWithValue("@thing_id", item.ThingId);
            command.Parameters.AddWithValue("@title", item.Title);
            command.Parameters.AddWithValue("@vote_count", item.VoteCount);
            command.Parameters.AddWithValue("@vote_average", item.VoteAverage);

            dbHelper.ExecuteCommand(command);
        }

        public void UpdateItem(BasicInfo item, DbHelperScope dbHelper)
        {
            string sqlStatement = SQLStatementsForCommon.UpdateItemById;

            using var command = new SqlCommand(sqlStatement);

            command.Parameters.AddWithValue("@title", item.Title);
            command.Parameters.AddWithValue("@vote_count", item.VoteCount);
            command.Parameters.AddWithValue("@vote_average", item.VoteAverage);
            command.Parameters.AddWithValue("@thing_id", item.ThingId);

            dbHelper.ExecuteCommand(command);
        }
    }
}

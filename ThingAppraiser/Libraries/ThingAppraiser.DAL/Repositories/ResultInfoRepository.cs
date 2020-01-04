using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Acolyte.Assertions;
using ThingAppraiser.DAL.DataBaseProviders;
using ThingAppraiser.DAL.Mappers;
using ThingAppraiser.DAL.Properties;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.DAL.Repositories
{
    public sealed class ResultInfoRepository : IResultRepository, IRepository<ResultInfo, Guid>,
        IRepositoryBase, ITagable, ITypeId
    {
        private readonly DataBaseOptions _dbSettings;

        public IReadOnlyList<string> Columns { get; } = new List<string>
        {
            "thing_id", "rating_id", "rating_value"
        };

        public string TableName { get; } = "[dbo].[results]";

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(ResultInfoRepository);

        #endregion

        #region ITypeId Implementation

        /// <inheritdoc />
        public Type TypeId { get; } = typeof(ResultInfo);

        #endregion


        public ResultInfoRepository(DataBaseOptions dbSettings)
        {
            _dbSettings = dbSettings.ThrowIfNull(nameof(dbSettings));
        }

        #region IRepository<ResultInfo, Guid> Implementation

        public bool Contains(Guid ratingId)
        {
            string sqlStatement = SQLStatementsForResults.CountItemsById;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var query = new SqlCommand(sqlStatement);

            query.Parameters.AddWithValue("@rating_id", ratingId);

            return dbHelper.GetScalar<int>(query) > 0;
        }

        public void InsertItem(ResultInfo item)
        {
            string sqlStatement = SQLStatementsForResults.InsertItem;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var command = new SqlCommand(sqlStatement);

            command.Parameters.AddWithValue("@thing_id", item.ThingId);
            command.Parameters.AddWithValue("@rating_value", item.RatingValue);
            command.Parameters.AddWithValue("@rating_id", item.RatingId);

            dbHelper.ExecuteCommand(command);
            dbHelper.Commit();
        }

        public ResultInfo GetItemById(Guid ratingId)
        {
            string sqlStatement = SQLStatementsForResults.SelectItemById;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var query = new SqlCommand(sqlStatement);

            query.Parameters.AddWithValue("@rating_id", ratingId);

            return dbHelper.GetItem(new ResultInfoMapper(), query);
        }

        public IReadOnlyList<ResultInfo> GetAllData()
        {
            string sqlStatement = SQLStatementsForResults.SelectAllItems;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var query = new SqlCommand(sqlStatement);

            return dbHelper.GetData(new ResultInfoMapper(), query);
        }

        public void UpdateItem(ResultInfo item)
        {
            string sqlStatement = SQLStatementsForResults.UpdateItemById;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var command = new SqlCommand(sqlStatement);

            command.Parameters.AddWithValue("@rating_value", item.RatingValue);
            command.Parameters.AddWithValue("@thing_id", item.ThingId);
            command.Parameters.AddWithValue("@rating_id", item.RatingId);

            dbHelper.ExecuteCommand(command);
            dbHelper.Commit();
        }

        public void DeleteItemById(Guid ratingId)
        {
            string sqlStatement = SQLStatementsForResults.DeleteItemById;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var command = new SqlCommand(sqlStatement);

            command.Parameters.AddWithValue("@rating_id", ratingId);

            dbHelper.ExecuteCommand(command);
            dbHelper.Commit();
        }

        public void DeleteAllData()
        {
            string sqlStatement = SQLStatementsForResults.DeleteAllItems;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var command = new SqlCommand(sqlStatement);

            dbHelper.ExecuteCommand(command);
            dbHelper.Commit();
        }

        #endregion

        #region IResultRepository

        public IReadOnlyList<ThingIdWithRating> GetOrderedRatingsValue(Guid ratingId)
        {
            string sqlStatement = SQLStatementsForResults.SelectItemsWithDescOrderingByRating;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var query = new SqlCommand(sqlStatement);

            query.Parameters.AddWithValue("@rating_id", ratingId);

            return dbHelper.GetData(new ThingIdWithRatingMapper(), query);
        }

        #endregion
    }
}

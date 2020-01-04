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
    public sealed class RatingRepository : IRatingRepository, IRepository<Rating, Guid>,
        IRepositoryBase, ITagable, ITypeId
    {
        private readonly DataBaseOptions _dbSettings;

        public IReadOnlyList<string> Columns { get; } = new List<string>
        {
            "rating_id", "rating_name"
        };

        public string TableName { get; } = "[dbo].[ratings]";

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(RatingRepository);

        #endregion

        #region ITypeId Implementation

        /// <inheritdoc />
        public Type TypeId { get; } = typeof(Rating);

        #endregion


        public RatingRepository(DataBaseOptions dbSettings)
        {
            _dbSettings = dbSettings.ThrowIfNull(nameof(dbSettings));
        }

        #region IRepository<CRating, Guid> Implementation

        public bool Contains(Guid ratingId)
        {
            string sqlStatement = SQLStatementsForRatings.CountItemsById;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var query = new SqlCommand(sqlStatement);

            query.Parameters.AddWithValue("@rating_id", ratingId);

            return dbHelper.GetScalar<int>(query) > 0;
        }

        public void InsertItem(Rating item)
        {
            string sqlStatement = SQLStatementsForRatings.InsertItem;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var command = new SqlCommand(sqlStatement);

            command.Parameters.AddWithValue("@rating_id", item.RatingId);
            command.Parameters.AddWithValue("@rating_name", item.RatingName);

            dbHelper.ExecuteCommand(command);
            dbHelper.Commit();
        }

        public Rating GetItemById(Guid ratingId)
        {
            string sqlStatement = SQLStatementsForRatings.SelectItemById;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var query = new SqlCommand(sqlStatement);

            query.Parameters.AddWithValue("@rating_id", ratingId);

            return dbHelper.GetItem(new RatingMapper(), query);
        }

        public IReadOnlyList<Rating> GetAllData()
        {
            string sqlStatement = SQLStatementsForRatings.SelectAllItems;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var query = new SqlCommand(sqlStatement);

            return dbHelper.GetData(new RatingMapper(), query);
        }

        public void UpdateItem(Rating item)
        {
            string sqlStatement = SQLStatementsForRatings.UpdateItemById;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var command = new SqlCommand(sqlStatement);

            command.Parameters.AddWithValue("@rating_name", item.RatingName);
            command.Parameters.AddWithValue("@rating_id", item.RatingId);

            dbHelper.ExecuteCommand(command);
            dbHelper.Commit();
        }

        public void DeleteItemById(Guid ratingId)
        {
            string sqlStatement = SQLStatementsForRatings.DeleteItemById;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var command = new SqlCommand(sqlStatement);

            command.Parameters.AddWithValue("@rating_id", ratingId);

            dbHelper.ExecuteCommand(command);
            dbHelper.Commit();
        }

        public void DeleteAllData()
        {
            string sqlStatement = SQLStatementsForRatings.DeleteAllItems;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var command = new SqlCommand(sqlStatement);

            dbHelper.ExecuteCommand(command);
            dbHelper.Commit();
        }

        #endregion
    }
}

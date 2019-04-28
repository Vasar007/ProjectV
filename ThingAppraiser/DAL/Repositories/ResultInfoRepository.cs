using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ThingAppraiser.Data;
using ThingAppraiser.DAL.Mappers;

namespace ThingAppraiser.DAL.Repositories
{
    public class CResultInfoRepository : IResultRepository, IRepository<CResultInfo, Guid>, 
        ITagable, ITypeID
    {
        private readonly CDataStorageSettings _dbSettings;

        public IReadOnlyList<String> Columns { get; } = new List<String>
        {
            "thing_id", "rating_id", "rating_value"
        };

        #region ITagable Implementation

        /// <inheritdoc />
        public String Tag => "ResultInfoRepository";

        #endregion

        #region ITypeID Implementation

        /// <inheritdoc />
        public Type TypeID => typeof(CResultInfo);

        #endregion


        public CResultInfoRepository(CDataStorageSettings dbSettings)
        {
            _dbSettings = dbSettings.ThrowIfNull(nameof(dbSettings));
        }

        #region IRepository<CResultInfo, Guid> Implementation

        public Boolean Contains(Guid ratingID)
        {
            String sqlStatement = "SELECT COUNT(*) " +
                                  "FROM [dbo].[results] " +
                                  "WHERE thing_id = @thing_id";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var query = new SqlCommand(sqlStatement))
            {
                query.Parameters.AddWithValue("@thing_id", ratingID);

                return dbHelper.GetScalar<Int32>(query) > 0;
            }
        }

        public void InsertItem(CResultInfo item)
        {
            String sqlStatement = "INSERT INTO [dbo].[results] " +
                                  "(thing_id, rating_value, rating_id) " +
                                  "VALUES (@thing_id, @rating_value, @rating_id)";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var command = new SqlCommand(sqlStatement))
            {
                command.Parameters.AddWithValue("@thing_id", item.ThingID);
                command.Parameters.AddWithValue("@rating_value", item.RatingValue);
                command.Parameters.AddWithValue("@rating_id", item.RatingID);

                dbHelper.ExecuteCommand(command);
                dbHelper.Commit();
            }
        }

        public CResultInfo GetItemByID(Guid ratingID)
        {
            String sqlStatement = "SELECT thing_id, rating_value, rating_id " +
                                  "FROM [dbo].[results] " +
                                  "WHERE rating_id = @rating_id";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var query = new SqlCommand(sqlStatement))
            {
                query.Parameters.AddWithValue("@rating_id", ratingID);

                return dbHelper.GetItem(new CResultInfoMapper(), query);
            }
        }

        public List<CResultInfo> GetAllData()
        {
            String sqlStatement = "SELECT thing_id, rating_value, rating_id " +
                                  "FROM [dbo].[results]";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var query = new SqlCommand(sqlStatement))
            {
                return dbHelper.GetData(new CResultInfoMapper(), query);
            }
        }

        public void UpdateItem(CResultInfo item)
        {
            String sqlStatement = "UPDATE [dbo].[results] " +
                                  "SET rating_value = @rating_value " +
                                  "WHERE thing_id = @thing_id AND rating_id = @rating_id";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var command = new SqlCommand(sqlStatement))
            {
                command.Parameters.AddWithValue("@rating_value", item.RatingValue);
                command.Parameters.AddWithValue("@thing_id", item.ThingID);
                command.Parameters.AddWithValue("@rating_id", item.RatingID);

                dbHelper.ExecuteCommand(command);
                dbHelper.Commit();
            }
        }

        public void DeleteItemByID(Guid ratingID)
        {
            String sqlStatement = "DELETE " +
                                  "FROM [dbo].[results] " +
                                  "WHERE rating_id = @rating_id";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var command = new SqlCommand(sqlStatement))
            {
                command.Parameters.AddWithValue("@rating_id", ratingID);

                dbHelper.ExecuteCommand(command);
                dbHelper.Commit();
            }
        }

        public void DeleteAllData()
        {
            String sqlStatement = "DELETE " +
                                  "FROM [dbo].[results]";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var command = new SqlCommand(sqlStatement))
            {
                dbHelper.ExecuteCommand(command);
                dbHelper.Commit();
            }
        }

        #endregion

        #region IResultRepository

        public List<CThingIDWithRating> GetOrderedRatingsValue(Guid ratingID)
        {
            String sqlStatement = "SELECT thing_id, rating_value " +
                                  "FROM [dbo].[results] " +
                                  "WHERE rating_id = @rating_id " +
                                  "ORDER BY rating_value DESC";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var query = new SqlCommand(sqlStatement))
            {
                query.Parameters.AddWithValue("@rating_id", ratingID);

                return dbHelper.GetData(new CThingIDWithRatingMapper(), query);
            }
        }

        #endregion
    }
}

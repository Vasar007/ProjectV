using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ThingAppraiser.Data;
using ThingAppraiser.DAL.Mappers;

namespace ThingAppraiser.DAL.Repositories
{
    public class CRatingRepository : IRatingRepository, IRepository<CRating, Guid>, ITagable, 
        ITypeID
    {
        private readonly CDataStorageSettings _dbSettings;

        public IReadOnlyList<String> Columns { get; } = new List<String>
        {
            "rating_id", "rating_name"
        };

        #region ITagable Implementation

        /// <inheritdoc />
        public String Tag => "RatingRepository";

        #endregion

        #region ITypeID Implementation

        /// <inheritdoc />
        public Type TypeID => typeof(CRating);

        #endregion


        public CRatingRepository(CDataStorageSettings dbSettings)
        {
            _dbSettings = dbSettings.ThrowIfNull(nameof(dbSettings));
        }

        #region IRepository<CRating, Guid> Implementation

        public Boolean Contains(Guid id)
        {
            String sqlStatement = "SELECT COUNT(*) " +
                                  "FROM [dbo].[ratings] " +
                                  "WHERE rating_id = @rating_id";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var query = new SqlCommand(sqlStatement))
            {
                query.Parameters.AddWithValue("@rating_id", id);

                return dbHelper.GetScalar<Int32>(query) > 0;
            }
        }

        public void InsertItem(CRating item)
        {
            String sqlStatement = "INSERT INTO [dbo].[ratings] " +
                                  "(rating_id, rating_name) " +
                                  "VALUES (@rating_id, @rating_name)";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var command = new SqlCommand(sqlStatement))
            {
                command.Parameters.AddWithValue("@rating_id", item.RatingID);
                command.Parameters.AddWithValue("@rating_name", item.RatingName);

                dbHelper.ExecuteCommand(command);
                dbHelper.Commit();
            }
        }

        public CRating GetItemByID(Guid id)
        {
            String sqlStatement = "SELECT rating_id, rating_name " +
                                  "FROM [dbo].[ratings]";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var query = new SqlCommand(sqlStatement))
            {
                return dbHelper.GetItem(new CRatingMapper(), query);
            }
        }

        public List<CRating> GetAllData()
        {
            String sqlStatement = "SELECT rating_id, rating_name " +
                                  "FROM [dbo].[ratings]";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var query = new SqlCommand(sqlStatement))
            {
                return dbHelper.GetData(new CRatingMapper(), query);
            }
        }

        public void UpdateItem(CRating item)
        {
            String sqlStatement = "UPDATE [dbo].[ratings] " +
                                  "SET rating_name = @rating_name " +
                                  "WHERE rating_id = @rating_id";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var command = new SqlCommand(sqlStatement))
            {
                command.Parameters.AddWithValue("@rating_name", item.RatingName);
                command.Parameters.AddWithValue("@rating_id", item.RatingID);

                dbHelper.ExecuteCommand(command);
                dbHelper.Commit();
            }
        }

        public void DeleteItemByID(Guid ratingID)
        {
            String sqlStatement = "DELETE " +
                                  "FROM [dbo].[ratings] " +
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
                                  "FROM [dbo].[ratings]";
            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var command = new SqlCommand(sqlStatement))
            {
                dbHelper.ExecuteCommand(command);
                dbHelper.Commit();
            }
        }

        #endregion
    }
}

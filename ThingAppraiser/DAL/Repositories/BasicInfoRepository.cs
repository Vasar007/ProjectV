using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ThingAppraiser.DAL.Mappers;
using ThingAppraiser.Data;

namespace ThingAppraiser.DAL.Repositories
{
    public class CBasicInfoRepository : IDataRepository, IRepository<CBasicInfo, Int32>, ITagable, 
        ITypeID
    {
        private readonly CDataStorageSettings _dbSettings;

        private readonly CDataProcessor _dataProcessor;

        public IReadOnlyList<String> Columns { get; } = new List<String>
        {
            "thing_id", "title", "vote_count", "vote_average"
        };

        #region ITagable Implementation

        /// <inheritdoc />
        public String Tag => "BasicInfoRepository";

        #endregion

        #region ITypeID Implementation

        /// <inheritdoc />
        public Type TypeID => typeof(CBasicInfo);

        #endregion


        public CBasicInfoRepository(CDataStorageSettings dbSettings)
        {
            _dbSettings = dbSettings.ThrowIfNull(nameof(dbSettings));
            _dataProcessor = new CDataProcessor(_dbSettings);
        }

        #region IRepository Implementation

        public Boolean Contains(Int32 thingID)
        {
            String sqlStatement = "SELECT COUNT(*) " +
                                  "FROM [dbo].[common] " +
                                  "WHERE thing_id = @thing_id";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var query = new SqlCommand(sqlStatement))
            {
                query.Parameters.AddWithValue("@thing_id", thingID);

                return dbHelper.GetScalar<Int32>(query) > 0;
            }
        }

        public void InsertItem(CBasicInfo item)
        {
            using (var dbHelper = new CDBHelperScope(_dbSettings))
            {
                InsertItem(item, dbHelper);
                dbHelper.Commit();
            }
        }

        public CBasicInfo GetItemByID(Int32 thingID)
        {
            String sqlStatement = "SELECT thing_id, title, vote_count, vote_average " +
                                  "FROM [dbo].[common] " +
                                  "WHERE thing_id = @thing_id";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var query = new SqlCommand(sqlStatement))
            {
                query.Parameters.AddWithValue("@thing_id", thingID);

                return dbHelper.GetItem(new CBasicInfoMapper(), query);
            }
        }

        public List<CBasicInfo> GetAllData()
        {
            String sqlStatement = "SELECT thing_id, title, vote_count, vote_average " +
                                  "FROM [dbo].[common]";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var query = new SqlCommand(sqlStatement))
            {
                return dbHelper.GetData(new CBasicInfoMapper(), query);
            }
        }

        public void UpdateItem(CBasicInfo item)
        {
            using (var dbHelper = new CDBHelperScope(_dbSettings))
            {
                UpdateItem(item, dbHelper);
                dbHelper.Commit();
            }
        }

        public void DeleteItemByID(Int32 thingID)
        {
            String sqlStatement = "DELETE " +
                                  "FROM [dbo].[common] " +
                                  "WHERE thing_id = @thing_id";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var command = new SqlCommand(sqlStatement))
            {
                command.Parameters.AddWithValue("@thing_id", thingID);

                dbHelper.ExecuteCommand(command);
                dbHelper.Commit();
            }
        }

        public void DeleteAllData()
        {
            String sqlStatement = "DELETE " +
                                  "FROM [dbo].[common]";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var command = new SqlCommand(sqlStatement))
            {
                dbHelper.ExecuteCommand(command);
                dbHelper.Commit();
            }
        }

        #endregion

        #region IDataRepository Implementation

        public T GetMinimum<T>(String columnName)
        {
            if (!Columns.Contains(columnName)) return default(T);
            return _dataProcessor.GetMinimum<T>(columnName, "[dbo].[common]");
        }

        public T GetMaximum<T>(String columnName)
        {
            if (!Columns.Contains(columnName)) return default(T);
            return _dataProcessor.GetMaximum<T>(columnName, "[dbo].[common]");
        }

        public (T, T) GetMinMax<T>(String columnName)
        {
            if (!Columns.Contains(columnName)) return (default(T), default(T));
            return _dataProcessor.GetMinMax<T>(columnName, "[dbo].[common]");
        }

        #endregion

        public void InsertItem(CBasicInfo item, CDBHelperScope dbHelper)
        {
            String sqlStatement = "INSERT INTO [dbo].[common] " +
                                  "(thing_id, title, vote_count, vote_average) " +
                                  "VALUES (@thing_id, @title, @vote_count, @vote_average)";

            using (var command = new SqlCommand(sqlStatement))
            {
                command.Parameters.AddWithValue("@thing_id", item.ThingID);
                command.Parameters.AddWithValue("@title", item.Title);
                command.Parameters.AddWithValue("@vote_count", item.VoteCount);
                command.Parameters.AddWithValue("@vote_average", item.VoteAverage);

                dbHelper.ExecuteCommand(command);
            }
        }

        public void UpdateItem(CBasicInfo item, CDBHelperScope dbHelper)
        {
            String sqlStatement = "UPDATE [dbo].[common] " +
                                  "SET title = @title, vote_count = @vote_count, " +
                                      "vote_average = @vote_average " +
                                  "WHERE thing_id = @thing_id";

            using (var command = new SqlCommand(sqlStatement))
            {
                command.Parameters.AddWithValue("@title", item.Title);
                command.Parameters.AddWithValue("@vote_count", item.VoteCount);
                command.Parameters.AddWithValue("@vote_average", item.VoteAverage);
                command.Parameters.AddWithValue("@thing_id", item.ThingID);

                dbHelper.ExecuteCommand(command);
            }
        }
    }
}

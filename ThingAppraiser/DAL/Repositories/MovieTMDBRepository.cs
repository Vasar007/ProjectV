using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ThingAppraiser.DAL.Mappers;
using ThingAppraiser.Data;

namespace ThingAppraiser.DAL.Repositories
{
    public class CMovieTMDBRepository : IDataRepository, IRepository<CBasicInfo, Int32>, ITagable, 
        ITypeID
    {
        private readonly CDataStorageSettings _dbSettings;

        private readonly CBasicInfoRepository _basicInfoRepository;

        private readonly CDataProcessor _dataProcessor;

        public IReadOnlyList<String> Columns { get; } = new List<String>
        {
            "thing_id", "overview", "release_date", "popularity", "adult", "genre_ids",
            "poster_path"
        };

        #region ITagable Implementation

        /// <inheritdoc />
        public String Tag => "MovieTMDBRepository";

        #endregion

        #region ITypeID Implementation

        /// <inheritdoc />
        public Type TypeID => typeof(CMovieTMDBInfo);

        #endregion


        public CMovieTMDBRepository(CDataStorageSettings dbSettings)
        {
            _dbSettings = dbSettings;
            _basicInfoRepository = new CBasicInfoRepository(dbSettings);
            _dataProcessor = new CDataProcessor(_dbSettings);
        }

        #region IRepository<CBasicInfo, Int32> Implementation

        public Boolean Contains(Int32 thingID)
        {
            String sqlStatement = "SELECT COUNT(*) " +
                                  "FROM [dbo].[tmdb] " +
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
            String sqlStatement = "INSERT INTO [dbo].[tmdb] " +
                                  "(thing_id, overview, release_date, popularity, adult, " +
                                  "genre_ids, poster_path) " +
                                  "VALUES (@thing_id, @overview, @release_date, @popularity, " +
                                          "@adult, @genre_ids, @poster_path)";

            if (!(item is CMovieTMDBInfo movieInfo)) return;

            String genreIDsAsString = String.Join(",", movieInfo.GenreIDs);

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            {
                if (!_basicInfoRepository.Contains(item.ThingID))
                {
                    _basicInfoRepository.InsertItem(item, dbHelper);
                }

                using (var command = new SqlCommand(sqlStatement))
                {
                    command.Parameters.AddWithValue("@thing_id", movieInfo.ThingID);
                    command.Parameters.AddWithValue("@overview", movieInfo.Overview);
                    command.Parameters.AddWithValue("@release_date", movieInfo.ReleaseDate);
                    command.Parameters.AddWithValue("@popularity", movieInfo.Popularity);
                    command.Parameters.AddWithValue("@adult", movieInfo.Adult);
                    command.Parameters.AddWithValue("@genre_ids", genreIDsAsString);
                    command.Parameters.AddWithValue("@poster_path", movieInfo.PosterPath);

                    dbHelper.ExecuteCommand(command);
                }

                dbHelper.Commit();
            }
        }

        public CBasicInfo GetItemByID(Int32 thingID)
        {
            String sqlStatement = "SELECT [dbo].[common].thing_id, " +
                                         "[dbo].[common].title, " +
                                         "[dbo].[common].vote_count, " +
                                         "[dbo].[common].vote_average, " +
                                         "[dbo].[tmdb].overview, " +
                                         "[dbo].[tmdb].release_date, " +
                                         "[dbo].[tmdb].popularity, " +
                                         "[dbo].[tmdb].adult, " +
                                         "[dbo].[tmdb].genre_ids, " +
                                         "[dbo].[tmdb].poster_path " +
                                  "FROM [dbo].[common] " +
                                      "INNER JOIN [dbo].[tmdb] ON " +
                                          "[dbo].[common].thing_id = " +
                                          "[dbo].[tmdb].thing_id " +
                                  "WHERE [dbo].[common].thing_id = @thing_id";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var query = new SqlCommand(sqlStatement))
            {
                query.Parameters.AddWithValue("@thing_id", thingID);

                return dbHelper.GetItem(new CMovieTMDBMapper(), query);
            }
        }

        public List<CBasicInfo> GetAllData()
        {
            String sqlStatement = "SELECT [dbo].[common].thing_id, " +
                                         "[dbo].[common].title, " +
                                         "[dbo].[common].vote_count, " +
                                         "[dbo].[common].vote_average, " +
                                         "[dbo].[tmdb].overview, " +
                                         "[dbo].[tmdb].release_date, " +
                                         "[dbo].[tmdb].popularity, " +
                                         "[dbo].[tmdb].adult, " +
                                         "[dbo].[tmdb].genre_ids, " +
                                         "[dbo].[tmdb].poster_path " +
                                  "FROM [dbo].[common] " +
                                      "INNER JOIN [dbo].[tmdb] ON " +
                                          "[dbo].[common].thing_id = " +
                                          "[dbo].[tmdb].thing_id";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            using (var query = new SqlCommand(sqlStatement))
            {
                return dbHelper.GetData(new CMovieTMDBMapper(), query);
            }
        }

        public void UpdateItem(CBasicInfo item)
        {
            String sqlStatement = "UPDATE [dbo].[tmdb] " +
                                  "SET overview = @overview, release_date = @release_date, " +
                                      "popularity = @popularity, adult = @adult, " +
                                      "genre_ids = @genre_ids, poster_path = @poster_path " +
                                  "WHERE thing_id = @thing_id";

            if (!(item is CMovieTMDBInfo movieInfo)) return;

            String genreIDsAsString = String.Join(",", movieInfo.GenreIDs);

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            {
                _basicInfoRepository.UpdateItem(item, dbHelper);

                using (var command = new SqlCommand(sqlStatement))
                {
                    command.Parameters.AddWithValue("@overview", movieInfo.Overview);
                    command.Parameters.AddWithValue("@release_date", movieInfo.ReleaseDate);
                    command.Parameters.AddWithValue("@popularity", movieInfo.Popularity);
                    command.Parameters.AddWithValue("@adult", movieInfo.Adult);
                    command.Parameters.AddWithValue("@genre_ids", genreIDsAsString);
                    command.Parameters.AddWithValue("@poster_path", movieInfo.PosterPath);
                    command.Parameters.AddWithValue("@thing_id", movieInfo.ThingID);

                    dbHelper.ExecuteCommand(command);
                }

                dbHelper.Commit();
            }
        }

        public void DeleteAllData()
        {
            String sqlStatement = "DELETE " +
                                  "FROM [dbo].[tmdb]";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            {
                using (var command = new SqlCommand(sqlStatement))
                {
                    dbHelper.ExecuteCommand(command);
                }

                dbHelper.Commit();
            }
        }

        public void DeleteItemByID(Int32 thingID)
        {
            String sqlStatement = "DELETE " +
                                  "FROM [dbo].[tmdb] " +
                                  "WHERE thing_id = @thing_id";

            using (var dbHelper = new CDBHelperScope(_dbSettings))
            {
                using (var command = new SqlCommand(sqlStatement))
                {
                    command.Parameters.AddWithValue("@thing_id", thingID);

                    dbHelper.ExecuteCommand(command);
                }

                dbHelper.Commit();
            }
        }

        #endregion

        #region IDataRepository Implementation

        public T GetMinimum<T>(String columnName)
        {
            if (!Columns.Contains(columnName)) return default(T);
            if (_basicInfoRepository.Columns.Contains(columnName))
            {
                return _basicInfoRepository.GetMinimum<T>(columnName);
            }
            return _dataProcessor.GetMinimum<T>(columnName, "[dbo].[tmdb]");
        }

        public T GetMaximum<T>(String columnName)
        {
            if (_basicInfoRepository.Columns.Contains(columnName))
            {
                return _basicInfoRepository.GetMaximum<T>(columnName);
            }
            if (!Columns.Contains(columnName)) return default(T);
            return _dataProcessor.GetMaximum<T>(columnName, "[dbo].[tmdb]");
        }

        public (T, T) GetMinMax<T>(String columnName)
        {
            if (_basicInfoRepository.Columns.Contains(columnName))
            {
                return _basicInfoRepository.GetMinMax<T>(columnName);
            }
            if (!Columns.Contains(columnName)) return (default(T), default(T));
            return _dataProcessor.GetMinMax<T>(columnName, "[dbo].[tmdb]");
        }

        #endregion
    }
}

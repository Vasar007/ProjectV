using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ProjectV.DataAccessLayer.DataBaseProviders;
using ProjectV.DataAccessLayer.Mappers;
using ProjectV.DataAccessLayer.Properties;
using ProjectV.Models.Data;

namespace ProjectV.DataAccessLayer.Repositories
{
    public sealed class TmdbMovieRepository : IDataRepository, IRepository<BasicInfo, int>, 
        IRepositoryBase, ITagable, ITypeId
    {
        private readonly DataBaseOptions _dbSettings;

        private readonly BasicInfoRepository _basicInfoRepository;

        private readonly DataProcessor _dataProcessor;

        public IReadOnlyList<string> Columns { get; } = new List<string>
        {
            "thing_id", "overview", "release_date", "popularity", "adult", "genre_ids",
            "poster_path"
        };

        public string TableName { get; } = "[dbo].[tmdb]";

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(TmdbMovieRepository);

        #endregion

        #region ITypeId Implementation

        /// <inheritdoc />
        public Type TypeId { get; } = typeof(TmdbMovieInfo);

        #endregion


        public TmdbMovieRepository(DataBaseOptions dbSettings)
        {
            _dbSettings = dbSettings;
            _basicInfoRepository = new BasicInfoRepository(dbSettings);
            _dataProcessor = new DataProcessor(_dbSettings);
        }

        #region IRepository<BasicInfo, int> Implementation

        public bool Contains(int thingId)
        {
            string sqlStatement = SQLStatementsForTmdb.CountItemsById;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var query = new SqlCommand(sqlStatement);

            query.Parameters.AddWithValue("@thing_id", thingId);

            return dbHelper.GetScalar<int>(query) > 0;
        }

        public void InsertItem(BasicInfo item)
        {
            string sqlStatement = SQLStatementsForTmdb.InsertItem;

            if (!(item is TmdbMovieInfo movieInfo)) return;

            string genreIdsAsstring = string.Join(",", movieInfo.GenreIds);

            using var dbHelper = new DbHelperScope(_dbSettings);

            if (!_basicInfoRepository.Contains(item.ThingId))
            {
                _basicInfoRepository.InsertItem(item, dbHelper);
            }

            using (var command = new SqlCommand(sqlStatement))
            {
                command.Parameters.AddWithValue("@thing_id", movieInfo.ThingId);
                command.Parameters.AddWithValue("@overview", movieInfo.Overview);
                command.Parameters.AddWithValue("@release_date", movieInfo.ReleaseDate);
                command.Parameters.AddWithValue("@popularity", movieInfo.Popularity);
                command.Parameters.AddWithValue("@adult", movieInfo.Adult);
                command.Parameters.AddWithValue("@genre_ids", genreIdsAsstring);
                command.Parameters.AddWithValue("@poster_path", movieInfo.PosterPath);

                dbHelper.ExecuteCommand(command);
            }

            dbHelper.Commit();
        }

        public BasicInfo GetItemById(int thingId)
        {
            string sqlStatement = SQLStatementsForTmdb.SelectItemById;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var query = new SqlCommand(sqlStatement);

            query.Parameters.AddWithValue("@thing_id", thingId);

            return dbHelper.GetItem(new TmdbMovieMapper(), query);
        }

        public IReadOnlyList<BasicInfo> GetAllData()
        {
            string sqlStatement = SQLStatementsForTmdb.SelectAllItems;

            using var dbHelper = new DbHelperScope(_dbSettings);
            using var query = new SqlCommand(sqlStatement);

            return dbHelper.GetData(new TmdbMovieMapper(), query);
        }

        public void UpdateItem(BasicInfo item)
        {
            string sqlStatement = SQLStatementsForTmdb.UpdateItemById;

            if (!(item is TmdbMovieInfo movieInfo)) return;

            string genreIdsAsstring = string.Join(",", movieInfo.GenreIds);

            using var dbHelper = new DbHelperScope(_dbSettings);

            _basicInfoRepository.UpdateItem(item, dbHelper);

            using (var command = new SqlCommand(sqlStatement))
            {
                command.Parameters.AddWithValue("@overview", movieInfo.Overview);
                command.Parameters.AddWithValue("@release_date", movieInfo.ReleaseDate);
                command.Parameters.AddWithValue("@popularity", movieInfo.Popularity);
                command.Parameters.AddWithValue("@adult", movieInfo.Adult);
                command.Parameters.AddWithValue("@genre_ids", genreIdsAsstring);
                command.Parameters.AddWithValue("@poster_path", movieInfo.PosterPath);
                command.Parameters.AddWithValue("@thing_id", movieInfo.ThingId);

                dbHelper.ExecuteCommand(command);
            }

            dbHelper.Commit();
        }

        public void DeleteAllData()
        {
            string sqlStatement = SQLStatementsForTmdb.DeleteAllItems;

            using var dbHelper = new DbHelperScope(_dbSettings);

            using (var command = new SqlCommand(sqlStatement))
            {
                dbHelper.ExecuteCommand(command);
            }

            dbHelper.Commit();
        }

        public void DeleteItemById(int thingId)
        {
            string sqlStatement = SQLStatementsForTmdb.DeleteItemById;

            using var dbHelper = new DbHelperScope(_dbSettings);

            using (var command = new SqlCommand(sqlStatement))
            {
                command.Parameters.AddWithValue("@thing_id", thingId);

                dbHelper.ExecuteCommand(command);
            }

            dbHelper.Commit();
        }

        #endregion

        #region IDataRepository Implementation

        public T GetMinimum<T>(string columnName) 
            where T : struct
        {
            if (!Columns.Contains(columnName)) return default;
            if (_basicInfoRepository.Columns.Contains(columnName))
            {
                return _basicInfoRepository.GetMinimum<T>(columnName);
            }
            return _dataProcessor.GetMinimum<T>(columnName, TableName);
        }

        public T GetMaximum<T>(string columnName) 
            where T : struct
        {
            if (_basicInfoRepository.Columns.Contains(columnName))
            {
                return _basicInfoRepository.GetMaximum<T>(columnName);
            }
            if (!Columns.Contains(columnName)) return default;
            return _dataProcessor.GetMaximum<T>(columnName, TableName);
        }

        public (T, T) GetMinMax<T>(string columnName) 
            where T : struct
        {
            if (_basicInfoRepository.Columns.Contains(columnName))
            {
                return _basicInfoRepository.GetMinMax<T>(columnName);
            }
            if (!Columns.Contains(columnName)) return (default, default);
            return _dataProcessor.GetMinMax<T>(columnName, TableName);
        }

        #endregion
    }
}

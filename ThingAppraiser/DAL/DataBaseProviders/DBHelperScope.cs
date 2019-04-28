using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Transactions;
using ThingAppraiser.DAL.Mappers;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DAL
{
    public class CDBHelperScope : IDisposable
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CDBHelperScope>();

        private readonly SqlConnection _connection;

        private readonly TransactionScope _transaction;

        private Boolean _disposedValue;


        public CDBHelperScope(CDataStorageSettings settings)
        {
            settings.ThrowIfNull(nameof(settings));
            settings.DBConnectionString.ThrowIfNull(nameof(settings.DBConnectionString));

            try
            {
                _connection = new SqlConnection(settings.DBConnectionString);
                _transaction = new TransactionScope();
            }
            catch (Exception ex)
            {
                Dispose();
                s_logger.Error(ex, "Exception occurred during connection to database.");
                throw;
            }
        }

        private static List<T> GetData<T>(IDbConnection connection, IMapper<T> mapper,
            IDbCommand query)
        {
            var result = new List<T>();
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                query.Connection = connection;

                using (IDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(mapper.ReadItem(reader));
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                s_logger.Error(
                    ex, "Exception occurred during database query execution (array of data)."
                );
                throw;
            }
            return result;
        }

        private static T GetItem<T>(IDbConnection connection, IMapper<T> mapper, IDbCommand query)
        {
            T result;
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                query.Connection = connection;

                using (IDataReader reader = query.ExecuteReader())
                {
                    reader.Read();
                    result = mapper.ReadItem(reader);
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                s_logger.Error(
                    ex, "Exception occurred during database query execution (single item)."
                );
                throw;
            }
            return result;
        }

        private static T GetScalar<T>(IDbConnection connection, IDbCommand query)
        {
            T result;
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                query.Connection = connection;

                result = (T) query.ExecuteScalar();
            }
            catch (Exception ex)
            {
                s_logger.Error(
                    ex, "Exception occurred during database query execution (scalar value)."
                );
                throw;
            }
            return result;
        }

        private static Int32 ExecuteCommand(IDbConnection connection, IDbCommand command)
        {
            Int32 recordsAffected;
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                command.Connection = connection;

                recordsAffected = command.ExecuteNonQuery();
                s_logger.Info($"Changed {recordsAffected} records in DB.");
            }
            catch (SqlException ex)
            {
                s_logger.Error(
                    ex, "Exception occurred during database query execution (just command)."
                );
                throw;
            }
            return recordsAffected;
        }

        public List<T> GetData<T>(IMapper<T> mapper, IDbCommand query)
        {
            mapper.ThrowIfNull(nameof(mapper));
            query.ThrowIfNull(nameof(query));
            return GetData(_connection, mapper, query);
        }

        public T GetItem<T>(IMapper<T> mapper, IDbCommand query)
        {
            mapper.ThrowIfNull(nameof(mapper));
            query.ThrowIfNull(nameof(query));
            return GetItem(_connection, mapper, query);
        }

        public T GetScalar<T>(IDbCommand query)
        {
            query.ThrowIfNull(nameof(query));
            return GetScalar<T>(_connection, query);
        }

        public Int32 ExecuteCommand(IDbCommand command)
        {
            command.ThrowIfNull(nameof(command));
            return ExecuteCommand(_connection, command);
        }

        public void Commit()
        {
            _transaction?.Complete();
        }

        public DbConnection GetDBConnection()
        {
            return _connection;
        }

        #region IDisposable Implementation

        public void Dispose()
        {
            if (!_disposedValue)
            {
                _disposedValue = true;

                _transaction?.Dispose();

                _connection?.Close();
                _connection?.Dispose();
            }
        }

        #endregion
    }
}

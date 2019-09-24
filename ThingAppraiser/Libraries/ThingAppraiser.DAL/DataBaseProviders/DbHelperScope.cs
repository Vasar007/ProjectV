using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using ThingAppraiser.DAL.Mappers;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DAL.DataBaseProviders
{
    // TODO: add command creation method (important).
    public sealed class DbHelperScope : IDisposable
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<DbHelperScope>();

        private readonly SqlConnection _connection;

        private readonly SqlTransaction _transaction;

        private bool _disposed;


        public DbHelperScope(DataStorageSettings settings)
        {
            settings.ThrowIfNull(nameof(settings));
            settings.DbConnectionString.ThrowIfNull(nameof(settings.DbConnectionString));

            try
            {
                _connection = new SqlConnection(settings.DbConnectionString);
                _connection.Open();
                _transaction = _connection.BeginTransaction();
            }
            catch (Exception ex)
            {
                Dispose();
                _logger.Error(ex, "Exception occurred during connection to database.");
                throw;
            }
        }

        public IReadOnlyList<T> GetData<T>(IMapper<T> mapper, IDbCommand query)
        {
            mapper.ThrowIfNull(nameof(mapper));
            query.ThrowIfNull(nameof(query));
            return GetData(_connection, mapper, query, _transaction);
        }

        public T GetItem<T>(IMapper<T> mapper, IDbCommand query)
        {
            mapper.ThrowIfNull(nameof(mapper));
            query.ThrowIfNull(nameof(query));
            return GetItem(_connection, mapper, query, _transaction);
        }

        public T GetScalar<T>(IDbCommand query)
        {
            query.ThrowIfNull(nameof(query));
            return GetScalar<T>(_connection, query, _transaction);
        }

        public int ExecuteCommand(IDbCommand command)
        {
            command.ThrowIfNull(nameof(command));
            return ExecuteCommand(_connection, command, _transaction);
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public DbConnection GetDbConnection()
        {
            return _connection;
        }

        #region IDisposable Implementation

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _transaction?.Dispose();

            _connection?.Close();
            _connection?.Dispose();
        }

        #endregion

        private static void ConfigureCommand(IDbCommand command, IDbConnection connection,
            IDbTransaction transaction)
        {
            command.Connection = connection;
            command.Transaction = transaction;
        }

        private static IReadOnlyList<T> GetData<T>(IDbConnection connection, IMapper<T> mapper,
            IDbCommand query, IDbTransaction transaction)
        {
            var result = new List<T>();
            try
            {
                ConfigureCommand(query, connection, transaction);

                using IDataReader reader = query.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(mapper.ReadItem(reader));
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex, "Exception occurred during database query execution (array of data)."
                );
                throw;
            }
            return result;
        }

        private static T GetItem<T>(IDbConnection connection, IMapper<T> mapper, IDbCommand query,
            IDbTransaction transaction)
        {
            T result;
            try
            {
                ConfigureCommand(query, connection, transaction);

                using IDataReader reader = query.ExecuteReader();

                reader.Read();
                result = mapper.ReadItem(reader);

                reader.Close();
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex, "Exception occurred during database query execution (single item)."
                );
                throw;
            }
            return result;
        }

        private static T GetScalar<T>(IDbConnection connection, IDbCommand query,
            IDbTransaction transaction)
        {
            T result;
            try
            {
                ConfigureCommand(query, connection, transaction);

                result = (T) query.ExecuteScalar();
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex, "Exception occurred during database query execution (scalar value)."
                );
                throw;
            }
            return result;
        }

        private static int ExecuteCommand(IDbConnection connection, IDbCommand command,
            IDbTransaction transaction)
        {
            int recordsAffected;
            try
            {
                ConfigureCommand(command, connection, transaction);

                recordsAffected = command.ExecuteNonQuery();
                _logger.Info($"Changed {recordsAffected} records in DB.");
            }
            catch (SqlException ex)
            {
                _logger.Error(
                    ex, "Exception occurred during database query execution (just command)."
                );
                throw;
            }
            return recordsAffected;
        }
    }
}

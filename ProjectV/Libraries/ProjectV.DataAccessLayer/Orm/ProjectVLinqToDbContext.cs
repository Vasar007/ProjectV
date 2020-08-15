using System;
using System.Linq;
using System.Threading.Tasks;
using Acolyte.Assertions;
using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;
using LinqToDB.SchemaProvider;

namespace ProjectV.DataAccessLayer.Orm
{
    public sealed class ProjectVLinqToDbContext : DataConnection, IAsyncDisposable
    {
        public ITable<JobDbInfo> Jobs => GetTable<JobDbInfo>();

        public ProjectVLinqToDbContext(
            LinqToDbConnectionOptions<ProjectVLinqToDbContext> options)
        : base(options)
        {
        }

        public static ProjectVLinqToDbContext CreateForPostgreSQL(
            DatabaseOptions storageSettings)
        {
            storageSettings.ThrowIfNull(nameof(storageSettings));

            var option = new LinqToDbConnectionOptionsBuilder()
                .UsePostgreSQL(storageSettings.ConnectionString)
                .Build<ProjectVLinqToDbContext>();

            return new ProjectVLinqToDbContext(option);
        }

        public bool CreateTableIfNotExists(string tableName)
        {
            ISchemaProvider schemaProvider = DataProvider.GetSchemaProvider();
            DatabaseSchema dbSchema = schemaProvider.GetSchema(this);

            bool exists = dbSchema.Tables
                .Any(t => string.Equals(t.TableName, tableName, StringComparison.Ordinal));

            // Return false if table exists and true if it is created.
            if (exists) return false;

            this.CreateTable<JobDbInfo>();
            return true;
        }

        #region IAsyncDisposable Implementation

        /// <summary>
        /// Boolean flag used to show that object has already been disposed.
        /// </summary>
        private bool _disposed;

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            await DisposeAsync();

            _disposed = true;
        }

        #endregion
    }
}

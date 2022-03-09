using System;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectV.DataAccessLayer.Services.Jobs;

namespace ProjectV.DataAccessLayer
{
    public sealed class ProjectVDbContext : DbContext
    {
        private readonly DatabaseOptions _storageOptions;

        public DbSet<JobDbInfo>? Jobs { private get; set; }


        public ProjectVDbContext(DatabaseOptions storageOptions)
        {
            _storageOptions = storageOptions.ThrowIfNull(nameof(storageOptions));
        }

        public ProjectVDbContext(IOptions<DatabaseOptions> storageOptions)
        {
            _storageOptions = storageOptions.Value.ThrowIfNull(nameof(storageOptions));
        }

        public DbSet<JobDbInfo> GetJobDbSet()
        {
            if (Jobs is null)
            {
                throw new InvalidOperationException($"{nameof(Jobs)} DB set is not initialized.");
            }

            return Jobs;
        }

        public bool CanUseDb()
        {
            return _storageOptions.CanUseDatabase;
        }

        public async ValueTask ExecuteIfCanUseDb(Func<DbSet<JobDbInfo>, ValueTask> action)
        {
            action.ThrowIfNull(nameof(action));

            if (!CanUseDb())
            {
                return;
            }

            await action(GetJobDbSet());
        }

        public async ValueTask<TReturn?> ExecuteIfCanUseDb<TReturn>(
            Func<DbSet<JobDbInfo>, ValueTask<TReturn>> action)
        {
            action.ThrowIfNull(nameof(action));

            if (!CanUseDb())
            {
                return await ValueTask.FromResult<TReturn>(default!);
            }

            return await action(GetJobDbSet());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!CanUseDb())
            {
                return;
            }

            optionsBuilder
                .UseNpgsql(_storageOptions.ConnectionString, o => o.SetPostgresVersion(12, 0));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (!CanUseDb())
            {
                return;
            }

            // The next line is required for PostreSQL DB because
            // MSSQL default schema name is "dbo".
            modelBuilder.HasDefaultSchema("public");

            // Upload configurations from DAL assembly to work with immutable models.
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}

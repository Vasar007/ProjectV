using System;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql(_storageOptions.ConnectionString, o => o.SetPostgresVersion(12, 0));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // The next line is required for PostreSQL DB because
            // MSSQL default schema name is "dbo".
            modelBuilder.HasDefaultSchema("public");

            // Upload configurations from DAL assembly to work with immutable models.
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}

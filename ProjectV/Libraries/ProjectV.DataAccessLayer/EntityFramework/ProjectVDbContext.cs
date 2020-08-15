using System;
using Acolyte.Assertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ProjectV.DataAccessLayer.EntityFramework
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
            optionsBuilder.UseNpgsql(_storageOptions.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // The next line is required for PostreSQL DB because
            // MSSQL default schema name is "dbo".
            modelBuilder.HasDefaultSchema("public");
            base.OnModelCreating(modelBuilder);

            // Key fields have already mapped. Need to set the other fields.
            modelBuilder.Entity<JobDbInfo>(
                builder =>
                {
                    builder.HasKey(e => e.Id);
                    builder.Property(e => e.Name);
                    builder.Property(e => e.State);
                    builder.Property(e => e.Result);
                    builder.Property(e => e.Config);
                }
            );
        }
    }
}

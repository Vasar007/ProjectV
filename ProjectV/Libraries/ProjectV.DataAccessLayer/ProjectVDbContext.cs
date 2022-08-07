using System;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectV.Configuration;
using ProjectV.DataAccessLayer.Services.Jobs.Models;
using ProjectV.DataAccessLayer.Services.Tokens.Models;
using ProjectV.DataAccessLayer.Services.Users.Models;

namespace ProjectV.DataAccessLayer
{
    public sealed class ProjectVDbContext : DbContext
    {
        private readonly DatabaseOptions _storageOptions;

        public DbSet<JobDbInfo>? Jobs { private get; set; }

        public DbSet<UserDbInfo>? Users { private get; set; }

        public DbSet<RefreshTokenDbInfo>? Tokens { private get; set; }


        public ProjectVDbContext(
            DatabaseOptions storageOptions)
        {
            _storageOptions = storageOptions.ThrowIfNull(nameof(storageOptions));
        }

        public ProjectVDbContext(
            IOptions<DatabaseOptions> storageOptions)
            : this(storageOptions.GetCheckedValue())
        {
        }

        public DbSet<JobDbInfo> GetJobDbSet()
        {
            return Jobs switch
            {
                null => throw new InvalidOperationException($"{nameof(Jobs)} DB set is not initialized."),
                _ => Jobs
            };
        }

        public DbSet<UserDbInfo> GetUserDbSet()
        {
            return Users switch
            {
                null => throw new InvalidOperationException($"{nameof(Users)} DB set is not initialized."),
                _ => Users
            };
        }

        public DbSet<RefreshTokenDbInfo> GetTokenDbSet()
        {
            return Tokens switch
            {
                null => throw new InvalidOperationException($"{nameof(Tokens)} DB set is not initialized."),
                _ => Tokens
            };
        }

        public bool CanUseDb()
        {
            return _storageOptions.CanUseDatabase;
        }

        public async Task ExecuteIfCanUseDb<TDbInfo>(Func<DbSet<TDbInfo>> getDbSet,
            Func<DbSet<TDbInfo>, Task> action)
            where TDbInfo : class
        {
            getDbSet.ThrowIfNull(nameof(getDbSet));
            action.ThrowIfNull(nameof(action));

            if (!CanUseDb())
            {
                return;
            }

            await action(getDbSet());
        }

        public async Task<TReturn?> ExecuteIfCanUseDb<TDbInfo, TReturn>(
            Func<DbSet<TDbInfo>> getDbSet,
            Func<DbSet<TDbInfo>, Task<TReturn>> action)
            where TDbInfo : class
        {
            getDbSet.ThrowIfNull(nameof(getDbSet));
            action.ThrowIfNull(nameof(action));

            if (!CanUseDb())
            {
                return await Task.FromResult<TReturn?>(default);
            }

            return await action(getDbSet());
        }

        public async ValueTask ExecuteIfCanUseDb<TDbInfo>(Func<DbSet<TDbInfo>> getDbSet,
            Func<DbSet<TDbInfo>, ValueTask> action)
            where TDbInfo : class
        {
            getDbSet.ThrowIfNull(nameof(getDbSet));
            action.ThrowIfNull(nameof(action));

            if (!CanUseDb())
            {
                return;
            }

            await action(getDbSet());
        }

        public async ValueTask<TReturn?> ExecuteIfCanUseDb<TDbInfo, TReturn>(
            Func<DbSet<TDbInfo>> getDbSet,
            Func<DbSet<TDbInfo>, ValueTask<TReturn>> action)
            where TDbInfo : class
        {
            getDbSet.ThrowIfNull(nameof(getDbSet));
            action.ThrowIfNull(nameof(action));

            if (!CanUseDb())
            {
                return await ValueTask.FromResult<TReturn?>(default);
            }

            return await action(getDbSet());
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

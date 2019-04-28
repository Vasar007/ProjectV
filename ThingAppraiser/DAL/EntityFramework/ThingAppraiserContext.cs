using System;
using Microsoft.EntityFrameworkCore;
using ThingAppraiser.Data;

namespace ThingAppraiser.DAL.EntityFramework
{
    public class CThingAppraiserContext : DbContext
    {
        private readonly CDataStorageSettings _storageSettings;

        public DbSet<CBasicInfo> CommonData { get; set; }

        public DbSet<CMovieTMDBInfo> MoviesTMDB { get; set; }


        public CThingAppraiserContext(CDataStorageSettings storageSettings)
        {
            _storageSettings = storageSettings.ThrowIfNull(nameof(storageSettings));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_storageSettings.DBConnectionString); //"Data Source=thing_appraiser.db"
        }
    }
}

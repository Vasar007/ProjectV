using Microsoft.EntityFrameworkCore;
using ThingAppraiser.Data;

namespace ThingAppraiser.DAL.EntityFramework
{
    public class CThingAppraiserContext : DbContext
    {
        private readonly DataStorageSettings _storageSettings;

        public DbSet<BasicInfo> CommonData { get; set; }

        public DbSet<TmdbMovieInfo> TmdbMovies { get; set; }


        public CThingAppraiserContext(DataStorageSettings storageSettings)
        {
            _storageSettings = storageSettings.ThrowIfNull(nameof(storageSettings));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_storageSettings.DbConnectionString); //"Data Source=thing_appraiser.db"
        }
    }
}

using Microsoft.EntityFrameworkCore;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.DAL.EntityFramework
{
    public sealed class ThingAppraiserContext : DbContext
    {
        private readonly DataStorageSettings _storageSettings;

        public DbSet<BasicInfo> CommonData { get; set; }

        public DbSet<TmdbMovieInfo> TmdbMovies { get; set; }


        public ThingAppraiserContext(DataStorageSettings storageSettings)
        {
            _storageSettings = storageSettings.ThrowIfNull(nameof(storageSettings));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_storageSettings.DbConnectionString); //"Data Source=thing_appraiser.db"
        }
    }
}

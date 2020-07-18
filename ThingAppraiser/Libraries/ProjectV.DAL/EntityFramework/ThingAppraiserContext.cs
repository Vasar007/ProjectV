using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using ProjectV.Models.Data;

namespace ProjectV.DAL.EntityFramework
{
    // TODO: reseach option to use ORM (Entity Framework).
    public sealed class ProjectVContext : DbContext
    {
        //private readonly DataStorageSettings _storageSettings;

        public DbSet<BasicInfo> CommonData { get; set; } = default!;

        public DbSet<TmdbMovieInfo> TmdbMovies { get; set; } = default!;

        public DbSet<OmdbMovieInfo> OmdbMovies { get; set; } = default!;

        public DbSet<SteamGameInfo> SteamGames { get; set; } = default!;


        public ProjectVContext()
        {
            //_storageSettings = storageSettings.ThrowIfNull(nameof(storageSettings));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=thing_appraiser;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BasicInfo>(builder =>
            {
                builder.HasKey(e => e.ThingId);
                builder.Property(e => e.Title);
                builder.Property(e => e.VoteCount);
                builder.Property(e => e.VoteAverage);
            });

            modelBuilder.Entity<MovieInfo>(builder =>
            {
                builder.HasBaseType<BasicInfo>();
                builder.Property(e => e.Overview);
                builder.Property(e => e.ReleaseDate);
            });

            modelBuilder.Entity<TmdbMovieInfo>(builder =>
            {
                builder.HasBaseType<BasicInfo>();
                builder.HasBaseType<MovieInfo>();
                builder.Property(e => e.Popularity);
                builder.Property(e => e.Adult);
                builder.Property(e => e.GenreIds)
                        .HasConversion(
                            v => string.Join(",", v),
                            v => v.Split(new[] { ',' }, StringSplitOptions.None).Select(int.Parse).ToList()
                        );
                builder.Property(e => e.PosterPath);
            });

            modelBuilder.Entity<OmdbMovieInfo>(builder =>
            {
                builder.HasBaseType<BasicInfo>();
                builder.HasBaseType<MovieInfo>();
                builder.Property(e => e.Metascore);
                builder.Property(e => e.Rated);
                builder.Property(e => e.GenreIds)
                        .HasConversion(
                            v => string.Join(",", v),
                            v => v.Split(new[] { ',' }, StringSplitOptions.None).ToList()
                        );
                builder.Property(e => e.PosterPath);
            });

            modelBuilder.Entity<GameInfo>(builder =>
            {
                builder.HasBaseType<BasicInfo>();
                builder.Property(e => e.Overview);
                builder.Property(e => e.ReleaseDate);
            });

            modelBuilder.Entity<SteamGameInfo>(builder =>
            {
                builder.HasBaseType<BasicInfo>();
                builder.HasBaseType<GameInfo>();
                builder.Property(e => e.Price);
                builder.Property(e => e.RequiredAge);
                builder.Property(e => e.GenreIds)
                        .HasConversion(
                            v => string.Join(",", v),
                            v => v.Split(new[] { ',' }, StringSplitOptions.None).Select(int.Parse).ToList()
                        );
                builder.Property(e => e.PosterPath);
            });
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Design;

namespace ProjectV.DataAccessLayer
{
    /// <summary>
    /// Design-time factory used by <c>dotnet ef</c> tools (e.g. when running
    /// <c>migrations add</c>) to construct a <see cref="ProjectVDbContext" />
    /// without going through the application host. EF Core's design-time
    /// service provider cannot disambiguate between
    /// <see cref="ProjectVDbContext.ProjectVDbContext(DatabaseOptions)" /> and
    /// the <c>IOptions&lt;DatabaseOptions&gt;</c> overload, so an explicit
    /// factory is required.
    /// </summary>
    /// <remarks>
    /// Pulls connection details from the <c>DatabaseOptions__ConnectionString</c>
    /// and <c>DatabaseOptions__CanUseDatabase</c> environment variables. The
    /// design-time factory is invoked ONLY by the EF Core CLI; runtime DI
    /// continues to use the <see cref="Microsoft.Extensions.Options.IOptions{T}" />
    /// overload registered via <c>services.AddDbContext&lt;ProjectVDbContext&gt;()</c>
    /// in <c>ProjectV.ProcessingWebService.Startup</c>.
    /// </remarks>
    public sealed class ProjectVDbContextDesignTimeFactory
        : IDesignTimeDbContextFactory<ProjectVDbContext>
    {
        /// <inheritdoc />
        public ProjectVDbContext CreateDbContext(string[] args)
        {
            string connectionString =
                Environment.GetEnvironmentVariable("DatabaseOptions__ConnectionString")
                ?? throw new InvalidOperationException(
                    "Cannot resolve the PostgreSQL connection string required by the EF Core " +
                    "design-time factory. Set the DatabaseOptions__ConnectionString environment " +
                    "variable to a valid Npgsql connection string before running " +
                    "`dotnet ef migrations add` (or any other EF Core CLI command). " +
                    "Example: " +
                    "DatabaseOptions__ConnectionString=" +
                    "'Host=localhost;Port=5432;Database=ProjectV;Username=postgres;Password=postgres'");

            // CanUseDatabase MUST be true for the design-time factory; otherwise
            // ProjectVDbContext.OnConfiguring + OnModelCreating short-circuit and
            // the generated migration would be empty.
            var options = new DatabaseOptions(
                dbConnectionString: connectionString,
                canUseDatabase: true
            );

            return new ProjectVDbContext(options);
        }
    }
}

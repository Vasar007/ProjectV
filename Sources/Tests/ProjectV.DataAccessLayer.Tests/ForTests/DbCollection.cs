using Xunit;

namespace ProjectV.DataAccessLayer.Tests.ForTests
{
    /// <summary>
    /// xUnit collection definition that ties every DAL integration test class
    /// to a single shared <see cref="DbCollectionFixture" /> — a single
    /// Testcontainers PostgreSQL container is started once per assembly run,
    /// and every test class decorated with
    /// <c>[Collection(DbCollection.Name)]</c> joins it (Decision D-11).
    /// </summary>
    [CollectionDefinition(Name)]
    public sealed class DbCollection : ICollectionFixture<DbCollectionFixture>
    {
        /// <summary>
        /// Collection name used by <see cref="CollectionAttribute" /> on every
        /// DAL integration test class.
        /// </summary>
        public const string Name = "ProjectV.DAL.Db";
    }
}

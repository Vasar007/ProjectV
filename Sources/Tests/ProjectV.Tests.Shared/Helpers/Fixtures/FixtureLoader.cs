using System.IO;
using Acolyte.Assertions;

namespace ProjectV.Tests.Shared.Helpers.Fixtures
{
    /// <summary>
    /// Loads recorded JSON fixture files from
    /// <c>Sources/Tests/Fixtures/</c>. Used by contract tests
    /// (TMDb/OMDb/Steam) and any other suite that prefers static fixtures
    /// over in-memory mocks (Decision D-18).
    /// </summary>
    /// <remarks>
    /// Fixture files are copied to the test output directory at build time.
    /// At runtime the loader resolves them against
    /// <see cref="AppContext.BaseDirectory" /> + <c>Fixtures</c>.
    /// Naming convention: <c>{Provider}/{endpoint}-{scenario}.json</c>
    /// (for example <c>Tmdb/movie-by-id-success.json</c>).
    /// </remarks>
    public static class FixtureLoader
    {
        private static readonly string _fixturesRoot =
            Path.Combine(AppContext.BaseDirectory, "Fixtures");

        /// <summary>
        /// Reads and returns the raw JSON content of a recorded fixture file.
        /// </summary>
        /// <param name="relativeFixturePath">
        /// Path relative to <c>Sources/Tests/Fixtures/</c>, for example
        /// <c>Tmdb/movie-by-id-success.json</c>.
        /// </param>
        /// <returns>The fixture file contents as a string.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="relativeFixturePath" /> is
        /// <c>null</c>, empty, or whitespace.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the resolved fixture file does not exist on disk.
        /// </exception>
        public static string LoadJsonFixture(string relativeFixturePath)
        {
            relativeFixturePath.ThrowIfNullOrWhiteSpace(nameof(relativeFixturePath));

            string fullPath = Path.Combine(_fixturesRoot, relativeFixturePath);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException(
                    $"Fixture file not found: '{fullPath}'.", fullPath);
            }

            return File.ReadAllText(fullPath);
        }
    }
}

using System.Collections.Generic;

namespace ProjectV.IO.Input.File
{
    /// <summary>
    /// Represents common necessary methods to work with files.
    /// </summary>
    public interface IFileReaderAsync
    {
        /// <summary>
        /// Reads local file. File must satisfy a particular
        /// structure (it's implementation detail, see implementers remarks).
        /// </summary>
        /// <param name="filename">Filename to read.</param>
        /// <returns>Enumeration of entity names.</returns>
        IEnumerable<string> ReadFile(string filename);

        /// <summary>
        /// Reads local csv-file. File must satisfy a particular structure (it's implementation
        /// detail, see implementers remarks).
        /// </summary>
        /// <param name="filename">Filename to read</param>
        /// <returns>Enumeration of entity names.</returns>
        IEnumerable<string> ReadCsvFile(string filename);
    }
}

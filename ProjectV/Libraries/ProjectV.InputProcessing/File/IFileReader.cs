using System.Collections.Generic;

namespace ProjectV.IO.Input.File
{
    /// <summary>
    /// Represents common necessary methods to work with files.
    /// </summary>
    public interface IFileReader
    {
        /// <summary>
        /// Reads local file. File must satisfy a particular
        /// structure (it's implementation detail, see implementers remarks).
        /// </summary>
        /// <param name="filename">Filename to read.</param>
        /// <returns>Processed collection of entity names.</returns>
        List<string> ReadFile(string filename);

        /// <summary>
        /// Reads local csv-file. File must satisfy a particular structure (it's implementation
        /// detail, see implementers remarks).
        /// </summary>
        /// <param name="filename">Filename to read</param>
        /// <returns>Processed collection of entity names.</returns>
        List<string> ReadCsvFile(string filename);
    }
}

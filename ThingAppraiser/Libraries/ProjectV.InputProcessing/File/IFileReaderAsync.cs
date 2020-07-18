using System.Collections.Generic;

namespace ProjectV.IO.Input.File
{
    public interface IFileReaderAsync
    {
        IEnumerable<string> ReadFile(string filename);

        IEnumerable<string> ReadCsvFile(string filename);
    }
}

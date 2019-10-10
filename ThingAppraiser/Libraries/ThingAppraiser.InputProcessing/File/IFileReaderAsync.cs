using System.Collections.Generic;

namespace ThingAppraiser.IO.Input.File
{
    public interface IFileReaderAsync
    {
        IEnumerable<string> ReadFile(string filename);

        IEnumerable<string> ReadCsvFile(string filename);
    }
}

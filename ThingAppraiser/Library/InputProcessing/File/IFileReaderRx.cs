using System.Collections.Generic;

namespace ThingAppraiser.IO.Input
{
    public interface IFileReaderRx
    {
        IEnumerable<string> ReadFile(string filename);

        IEnumerable<string> ReadCsvFile(string filename);
    }
}

using System;
using System.Collections.Generic;

namespace ThingAppraiser.IO.Input
{
    public interface IFileReaderRx
    {
        IEnumerable<String> ReadFile(String filename);

        IEnumerable<String> ReadCsvFile(String filename);
    }
}

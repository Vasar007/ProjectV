using System;
using System.Collections.Generic;

namespace ThingAppraiser.IO.Output
{
    public interface IOutputterRx : ITagable
    {
        Boolean SaveResults(IEnumerable<COuputFileData> outputData, String storageName);
    }
}

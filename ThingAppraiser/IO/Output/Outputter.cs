using System.Collections.Generic;

namespace ThingAppraiser.IO.Output
{
    public interface IOutputter
    {
        bool SaveResults(List<List<Data.ResultType>> results, string storageName);
    }
}

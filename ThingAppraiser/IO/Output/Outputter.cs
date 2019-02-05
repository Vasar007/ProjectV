using System.Collections.Generic;

namespace ThingAppraiser.IO.Output
{
    public abstract class Outputter
    {
        public abstract bool SaveResults(List<List<Data.ResultType>> results, string storageName);
    }
}

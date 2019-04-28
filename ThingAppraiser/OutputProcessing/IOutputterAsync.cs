using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThingAppraiser.Data;

namespace ThingAppraiser.IO.Output
{
    public interface IOutputterAsync : ITagable
    {
        Task<Boolean> SaveResults(List<List<CRatingDataContainer>> resultsQueues, 
            String storageName);
    }
}

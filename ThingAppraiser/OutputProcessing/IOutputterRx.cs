using System.Collections.Generic;
using ThingAppraiser.Data;

namespace ThingAppraiser.IO.Output
{
    public interface IOutputterRx : IOutputterBase, ITagable
    {
        bool SaveResults(List<List<RatingDataContainer>> results, string storageName);
    }
}

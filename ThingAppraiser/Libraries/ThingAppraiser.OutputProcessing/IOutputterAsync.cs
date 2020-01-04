using System.Collections.Generic;
using System.Threading.Tasks;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.IO.Output
{
    public interface IOutputterAsync : IOutputterBase, ITagable
    {
        Task<bool> SaveResults(IReadOnlyList<IReadOnlyList<RatingDataContainer>> results,
            string storageName);
    }
}

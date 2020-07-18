using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectV.Models.Internal;

namespace ProjectV.IO.Output
{
    public interface IOutputterAsync : IOutputterBase, ITagable
    {
        Task<bool> SaveResults(IReadOnlyList<IReadOnlyList<RatingDataContainer>> results,
            string storageName);
    }
}

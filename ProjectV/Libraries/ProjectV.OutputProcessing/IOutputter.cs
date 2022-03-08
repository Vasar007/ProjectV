using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectV.Models.Internal;

namespace ProjectV.IO.Output
{
    /// <summary>
    /// Defines outputter classes interface to use in output component.
    /// </summary>
    public interface IOutputter : ITagable
    {
        /// <summary>
        /// Saves results of the processing to the storage. It could be NOT only direct writing to
        /// file, you can define your own output sources.
        /// </summary>
        /// <param name="results">Results to save.</param>
        /// <param name="storageName">Storage name in which should be saved results.</param>
        /// <returns>
        /// <c>true</c> if results were saved successfully, <c>false</c> otherwise.
        /// </returns>
        Task<bool> SaveResults(IReadOnlyList<IReadOnlyList<RatingDataContainer>> results,
            string storageName);
    }
}

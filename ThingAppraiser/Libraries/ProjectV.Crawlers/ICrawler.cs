using System.Collections.Generic;
using ProjectV.Models.Data;

namespace ProjectV.Crawlers
{
    /// <summary>
    /// Crawlers base interface for sequential service. You should inherit this class if would like
    /// to create your own crawler.
    /// </summary>
    public interface ICrawler : ICrawlerBase
    {
        /// <summary>
        /// Gets response from data storage and process it.
        /// </summary>
        /// <param name="entities">Names to find data.</param>
        /// <param name="outputResults">Flag to define need to output.</param>
        /// <returns>Transformed data to appraise.</returns>
        /// <remarks>
        /// Response collection must be unique because rating calculation errors can occur in such
        /// situations.
        /// </remarks>
        IReadOnlyList<BasicInfo> GetResponse(IReadOnlyList<string> entities, bool outputResults);
    }
}

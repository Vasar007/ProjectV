using System.Collections.Generic;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Crawlers
{
    /// <summary>
    /// Crawlers base class for sequential service. You should inherit this class if would like to 
    /// create your own crawler.
    /// </summary>
    public abstract class Crawler : CrawlerBase
    {
        #region ITagable Implementation

        /// <inheritdoc />
        public override string Tag { get; } = nameof(Crawler);

        #endregion


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        protected Crawler()
        {
        }

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
        public abstract IReadOnlyList<BasicInfo> GetResponse(IReadOnlyList<string> entities,
            bool outputResults);
    }
}

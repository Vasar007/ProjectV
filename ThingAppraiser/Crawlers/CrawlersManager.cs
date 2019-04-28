using System;
using System.Collections.Generic;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Crawlers
{
    /// <summary>
    /// Class to control all process of collecting data from services.
    /// </summary>
    public sealed class CCrawlersManager : IManager<CCrawler>
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CCrawlersManager>();

        /// <summary>
        /// Collection of concrete crawler implementations.
        /// </summary>
        private readonly List<CCrawler> _crawlers = new List<CCrawler>();

        /// <summary>
        /// Sets this flag to <c>true</c> if you need to monitor crawlers results.
        /// </summary>
        private readonly Boolean _outputResults;


        /// <summary>
        /// Initializes manager for crawlers.
        /// </summary>
        /// <param name="outputResults">Flag to define need to output crawlers results.</param>
        public CCrawlersManager(Boolean outputResults)
        {
            _outputResults = outputResults;
        }

        #region IManager<CCrawler> Implementation

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item">item</paramref> is <c>null</c>.
        /// </exception>
        public void Add(CCrawler item)
        {
            item.ThrowIfNull(nameof(item));
            if (!_crawlers.Contains(item))
            {
                _crawlers.Add(item);
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item">item</paramref> is <c>null</c>.
        /// </exception>
        public Boolean Remove(CCrawler item)
        {
            item.ThrowIfNull(nameof(item));
            return _crawlers.Remove(item);
        }

        #endregion

        /// <summary>
        /// Sends requests to all crawlers in collection and collect responses.
        /// </summary>
        /// <param name="entities">Collection of entities as strings to process.</param>
        /// <returns>Collection of results from crawlers produced from a set of entities.</returns>
        public List<List<CBasicInfo>> CollectAllResponses(List<String> entities)
        {
            var results = new List<List<CBasicInfo>>();
            foreach (CCrawler crawler in _crawlers)
            {
                results.Add(crawler.GetResponse(entities, _outputResults));
            }
            s_logger.Info("Crawlers have finished work.");
            return results;
        }
    }
}

using System;
using System.Collections.Generic;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Crawlers
{
    /// <summary>
    /// Class to control all process of collecting data from services.
    /// </summary>
    public sealed class CrawlersManager : IManager<Crawler>
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<CrawlersManager>();

        /// <summary>
        /// Collection of concrete crawler implementations.
        /// </summary>
        private readonly List<Crawler> _crawlers = new List<Crawler>();

        /// <summary>
        /// Sets this flag to <c>true</c> if you need to monitor crawlers results.
        /// </summary>
        private readonly bool _outputResults;


        /// <summary>
        /// Initializes manager for crawlers.
        /// </summary>
        /// <param name="outputResults">Flag to define need to output crawlers results.</param>
        public CrawlersManager(bool outputResults)
        {
            _outputResults = outputResults;
        }

        #region IManager<Crawler> Implementation

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item" /> is <c>null</c>.
        /// </exception>
        public void Add(Crawler item)
        {
            item.ThrowIfNull(nameof(item));
            if (!_crawlers.Contains(item))
            {
                _crawlers.Add(item);
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item" /> is <c>null</c>.
        /// </exception>
        public bool Remove(Crawler item)
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
        public IReadOnlyList<IReadOnlyList<BasicInfo>> CollectAllResponses(List<string> entities)
        {
            var results = new List<IReadOnlyList<BasicInfo>>();
            foreach (Crawler crawler in _crawlers)
            {
                results.Add(crawler.GetResponse(entities, _outputResults));
            }
            _logger.Info("Crawlers have finished work.");
            return results;
        }
    }
}

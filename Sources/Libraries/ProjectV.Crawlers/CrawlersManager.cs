using System;
using System.Collections.Generic;
using System.Linq;
using Acolyte.Assertions;
using ProjectV.DataPipeline;
using ProjectV.Logging;
using ProjectV.Models.Data;

namespace ProjectV.Crawlers
{
    /// <summary>
    /// Class to control all process of collecting data from services.
    /// </summary>
    public sealed class CrawlersManager : IManager<ICrawler>, IDisposable
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<CrawlersManager>();

        /// <summary>
        /// Collection of concrete crawler implementations.
        /// </summary>
        private readonly List<ICrawler> _crawlers = new List<ICrawler>();

        /// <summary>
        /// Sets this flag to <c>true</c> if you need to monitor crawlers results.
        /// </summary>
        private readonly bool _outputResults;


        /// <summary>
        /// Initializes manager for crawlers.
        /// </summary>
        /// <param name="outputResults">Flag to define need to output crawlers results.</param>
        public CrawlersManager(
            bool outputResults)
        {
            _outputResults = outputResults;
        }

        #region IManager<Crawler> Implementation

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item" /> is <c>null</c>.
        /// </exception>
        public void Add(ICrawler item)
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
        public bool Remove(ICrawler item)
        {
            item.ThrowIfNull(nameof(item));
            return _crawlers.Remove(item);
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Boolean flag used to show that object has already been disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Releases all resources used by crawlers.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            _crawlers.ForEach(crawlerAsync => crawlerAsync.Dispose());

            _disposed = true;
        }

        #endregion

        public CrawlersFlow CreateFlow()
        {
            var crawlersFunc = _crawlers.Select(crawler =>
                new Func<string, IAsyncEnumerable<BasicInfo>>(
                    entityName => TryGetResponse(crawler, entityName)
                )
            );

            var crawlersFlow = new CrawlersFlow(crawlersFunc);

            _logger.Info("Constructed crawlers pipeline.");
            return crawlersFlow;
        }


        /// <summary>
        /// Sends requests to crawler in collection and collect response(-s).
        /// </summary>
        /// <param name="entityName">Entity name to process.</param>
        /// <returns>Enumeration of results from crawler produced from an entitiy.</returns>
        private IAsyncEnumerable<BasicInfo> TryGetResponse(ICrawler crawler,
            string entityName)
        {
            try
            {
                return crawler.GetResponse(entityName, _outputResults);
            }
            catch (Exception ex)
            {
                string message = $"Crawler {crawler.Tag} could not process " +
                                 $"entity \"{entityName}\".";
                _logger.Error(ex, message);
                throw;
            }
        }
    }
}

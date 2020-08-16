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
    public sealed class CrawlersManagerAsync : IManager<ICrawlerAsync>, IDisposable
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<CrawlersManagerAsync>();

        /// <summary>
        /// Collection of concrete crawler implementations.
        /// </summary>
        private readonly List<ICrawlerAsync> _crawlersAsync = new List<ICrawlerAsync>();

        /// <summary>
        /// Sets this flag to <c>true</c> if you need to monitor crawlers results.
        /// </summary>
        private readonly bool _outputResults;


        /// <summary>
        /// Initializes manager for crawlers.
        /// </summary>
        /// <param name="outputResults">Flag to define need to output crawlers results.</param>
        public CrawlersManagerAsync(
            bool outputResults)
        {
            _outputResults = outputResults;
        }

        #region IManager<CrawlerAsync> Implementation

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item" /> is <c>null</c>.
        /// </exception>
        public void Add(ICrawlerAsync item)
        {
            item.ThrowIfNull(nameof(item));
            if (!_crawlersAsync.Contains(item))
            {
                _crawlersAsync.Add(item);
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item" /> is <c>null</c>.
        /// </exception>
        public bool Remove(ICrawlerAsync item)
        {
            item.ThrowIfNull(nameof(item));
            return _crawlersAsync.Remove(item);
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

            _crawlersAsync.ForEach(crawlerAsync => crawlerAsync.Dispose());

            _disposed = true;
        }

        #endregion

        public CrawlersFlow CreateFlow()
        {
            var crawlersFunc = _crawlersAsync.Select(crawlerAsync =>
                new Func<string, IAsyncEnumerable<BasicInfo>>(
                    entityName => TryGetResponse(crawlerAsync, entityName)
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
        private IAsyncEnumerable<BasicInfo> TryGetResponse(ICrawlerAsync crawlerAsync,
            string entityName)
        {
            try
            {
                return crawlerAsync.GetResponse(entityName, _outputResults);
            }
            catch (Exception ex)
            {
                string message = $"Crawler {crawlerAsync.Tag} could not process " +
                                 $"entity \"{entityName}\".";
                _logger.Error(ex, message);
                throw;
            }
        }
    }
}

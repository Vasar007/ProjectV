using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ThingAppraiser.DataPipeline;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Crawlers
{
    public sealed class CrawlersManagerAsync : IManager<ICrawlerAsync>, IDisposable
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<CrawlersManagerAsync>();

        private readonly List<ICrawlerAsync> _crawlersAsync = new List<ICrawlerAsync>();

        private readonly bool _outputResults;

        private bool _disposed;


        public CrawlersManagerAsync(bool outputResults)
        {
            _outputResults = outputResults;
        }

        #region IManager<CrawlerAsync> Implementation

        public void Add(ICrawlerAsync item)
        {
            item.ThrowIfNull(nameof(item));
            if (!_crawlersAsync.Contains(item))
            {
                _crawlersAsync.Add(item);
            }
        }

        public bool Remove(ICrawlerAsync item)
        {
            item.ThrowIfNull(nameof(item));
            return _crawlersAsync.Remove(item);
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _crawlersAsync.ForEach(crawlerAsync => crawlerAsync.Dispose());
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

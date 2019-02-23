using System.Collections;
using System.Collections.Generic;

namespace ThingAppraiser.Crawlers
{
    /// <summary>
    /// Class to controll all process of collecting data from services.
    /// </summary>
    /// <remarks>
    /// Class implements <see cref="IEnumerable"> interface to simplify call to the constructor 
    /// with collection initializer.
    /// </remarks>
    public class CrawlersManager : IEnumerable
    {
        #region Class Fields

        /// <summary>
        /// Collection of concrete crawler implementations.
        /// </summary>
        private List<Crawler> _crawlers = new List<Crawler>();

        #endregion

        #region Public Class Methods

        /// <summary>
        /// Add new crawler to collection.
        /// </summary>
        /// <param name="crawler">Concrete crawler implementation.</param>
        public void Add(Crawler crawler)
        {
            crawler.ThrowIfNull(nameof(crawler));
            _crawlers.Add(crawler);
        }

        /// <summary>
        /// Send requests to all crawlers in collection and collect responses.
        /// </summary>
        /// <param name="entities">Collection of entities as strings to process.</param>
        /// <returns>Collection of results from crawlers produced from a set of entities.</returns>
        public List<List<Data.DataHandler>> CollectAllResponses(List<string> entities)
        {
            var results = new List<List<Data.DataHandler>>();
            foreach (var crawler in _crawlers)
            {
                results.Add(crawler.GetResponse(entities));
            }
            return results;
        }

        #endregion

        #region Impements IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A collection enumerator.</returns>
        public IEnumerator GetEnumerator()
        {
            return _crawlers.GetEnumerator();
        }

        #endregion
    }
}

using System.Collections;
using System.Collections.Generic;

namespace ThingAppraiser.Crawlers
{
    public class CrawlersManager : IEnumerable
    {
        private List<Crawler> _crawlers = new List<Crawler>();

        public void Add(Crawler crawler)
        {
            crawler.ThrowIfNull(nameof(crawler));
            _crawlers.Add(crawler);
        }

        public List<List<Data.DataHandler>> GetAllData(List<string> entities)
        {
            var results = new List<List<Data.DataHandler>>();
            foreach (var crawler in _crawlers)
            {
                results.Add(crawler.GetData(entities));
            }
            return results;
        }

        public IEnumerator GetEnumerator()
        {
            return _crawlers.GetEnumerator();
        }
    }
}

using System.Collections.Generic;

namespace ThingAppraiser.Crawlers
{
    public class CrawlersManager
    {
        public List<Crawler> crawlers = new List<Crawler>();

        public void Add(Crawler crawler)
        {
            crawlers.Add(crawler);
        }

        public List<List<Data.DataHandler>> GetAllData(string[] entities)
        {
            var results = new List<List<Data.DataHandler>>();
            foreach (var crawler in crawlers)
            {
                results.Add(crawler.GetData(entities));
            }
            return results;
        }
    }
}

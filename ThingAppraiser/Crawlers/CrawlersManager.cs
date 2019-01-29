using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ThingAppraiser.Crawlers
{
    public class CrawlersManager : IEnumerable
    {
        private List<Crawler> _crawlers = new List<Crawler>();

        public void Add(Crawler crawler)
        {
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

        public static void PrintResultsToConsole(List<List<Data.DataHandler>> results)
        {
            foreach (var result in results)
            {
                foreach (var entity in result)
                {
                    Console.WriteLine(JToken.FromObject(entity));
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _crawlers.GetEnumerator();
        }
    }
}

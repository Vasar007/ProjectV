using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ThingAppraiser.Crawlers
{
    public class CrawlersManager : IEnumerable
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
            return crawlers.GetEnumerator();
        }
    }
}

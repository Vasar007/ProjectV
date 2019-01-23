using System;
using Newtonsoft.Json.Linq;

namespace ThingAppraiser
{
    class Program
    {
        static readonly string[] Movies = { "Allied", "Venom", "Sayonara no asa ni yakusoku no hana o kazaro" };

        static void Main(string[] args)
        {
            var crawler = new Crawlers.TMDBCrawler();
            var results = crawler.GetData(Movies);
            foreach (var result in results)
            {
                Console.WriteLine(JToken.FromObject(result));
            }
            Console.ReadLine();
        }
    }
}

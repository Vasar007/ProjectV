using System;
using Newtonsoft.Json.Linq;

namespace ThingAppraiser
{
    class Program
    {
        static readonly string[] Movies = { "Allied", "Venom", "Sayonara no asa ni yakusoku no hana o kazaro" };

        static void Main(string[] args)
        {
            var crawlerManager = new Crawlers.CrawlersManager();
            crawlerManager.Add(new Crawlers.TMDBCrawler());

            var results = crawlerManager.GetAllData(Movies);
            foreach (var result in results)
            {
                foreach (var entity in result)
                {
                    Console.WriteLine(JToken.FromObject(entity));
                }
            }
            Console.ReadLine();

            var appraisersManager = new Appraisers.AppraisersManager();
            appraisersManager.Add(new Appraisers.TMDBAppraiser());
            var ratings = appraisersManager.GetAllRatings(results);
            foreach (var rating in ratings)
            {
                foreach (var (item, value) in rating)
                {
                    Console.WriteLine($"{item.Title} has rating {value}.");
                }
            }
            Console.ReadLine();
        }
    }
}

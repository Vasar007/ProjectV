using System;

namespace ThingAppraiser
{
    public class Program
    {
        private static void Main(string[] args)
        {
            // Show the case when we have a movies to appraise.
            string[] names;
            if (args.Length == 1)
            {
                names = Input.Input.GetNamesFromFile(args[0]);
            }
            else
            {
                names = Input.Input.GetNamesFromFile();
            }

            if (names.Length == 0)
            {
                return;
            }

            var crawlerManager = new Crawlers.CrawlersManager
            {
                new Crawlers.TMDBCrawler()
            };
            var results = crawlerManager.GetAllData(names);
            Crawlers.CrawlersManager.PrintResultsToConsole(results);
            Console.ReadLine();

            var appraisersManager = new Appraisers.AppraisersManager
            {
                new Appraisers.TMDBAppraiser()
            };
            var ratings = appraisersManager.GetAllRatings(results);
            Appraisers.AppraisersManager.PrintRatingsToConsole(ratings);
            Console.ReadLine();
        }
    }
}

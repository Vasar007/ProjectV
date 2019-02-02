using System;
using System.Collections.Generic;

namespace ThingAppraiser
{
    public class Program
    {
        private static void Main(string[] args)
        {
            // Show the case when we have a movies to appraise.
            List<string> names;
            var inputManager = new Input.InputManager(new Input.GoogleDriveReader()); // LocalFileReader
            if (args.Length == 1)
            {
                names = inputManager.GetNames(args[0]);
            }
            else
            {
                Console.WriteLine("Enter filename which contains the Things:");
                names = inputManager.GetNames(Console.ReadLine());
            }

            if (names.Count == 0)
            {
                Console.WriteLine("Input is empty. Closing...");
                Console.ReadLine();
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

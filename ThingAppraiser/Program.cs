using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ThingAppraiser
{
    public class Program
    {
        private static void Main(string[] args)
        {
            // Show the case when we have a movies to appraise.
            List<string> names;
            var inputManager = new IO.Input.InputManager(new IO.Input.GoogleDriveReader()); // LocalFileReader
            if (args.Length == 1)
            {
                names = inputManager.GetNames(args[0]);
            }
            else
            {
                Console.Write("Enter filename which contains the Things: ");
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
            PrintResultsToConsole(results);
            Console.ReadLine();

            var appraisersManager = new Appraisers.AppraisersManager
            {
                new Appraisers.TMDBAppraiser()
            };
            var ratings = appraisersManager.GetAllRatings(results);
            PrintRatingsToConsole(ratings);
            Console.ReadLine();

            var outputManager = new IO.Output.OutputManager(new IO.Output.GoogleDriveWriter()); // LocalFileWriter
            if (outputManager.SaveResults(ratings))
            {
                Console.WriteLine("Ratings was saved to the file.");
            }
            else
            {
                Console.WriteLine("Ratings wasn't saved to the file.");
            }
            Console.ReadLine();
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

        public static void PrintRatingsToConsole(List<List<Data.ResultType>> ratings)
        {
            foreach (var rating in ratings)
            {
                foreach (var (item, value) in rating)
                {
                    Console.WriteLine($"{item.Title} has rating {value}.");
                }
            }
        }
    }

}

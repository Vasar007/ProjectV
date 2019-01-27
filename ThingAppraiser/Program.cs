using System;

namespace ThingAppraiser
{
    class Program
    {
        static readonly string[] Movies =
        {
                "Arrival",
                "Saving Private Ryan",
                "Kubo and the Two Strings",
                "Panfilov's 28 Men",
                "Kimi no suizo wo tabetai",
                "Expelled from Paradise",
                "Pawn Sacrifice",
                "Summer Wars",
                "The Shape of Water",
                "Blade Runner 2049",
                "The Gambler",
                "Maze Runner",
                "Pixels",
                "Edinichka",
                "Last Knights",
                "Ratchet & Clank",
                "The Mist",
                "Se7en",
                "Gone Girl",
                "Eternal Sunshine of the Spotless Mind"
        };

        private static void Main(string[] args)
        {
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
                names = Movies;
            }

            var crawlerManager = new Crawlers.CrawlersManager
            {
                new Crawlers.TMDBCrawler()
            };
            var results = crawlerManager.GetAllData(Movies);
            Crawlers.CrawlersManager.PrintResultsToConsole(results);
            Console.ReadLine();


            {
                new Appraisers.TMDBAppraiser()
            };
            var ratings = appraisersManager.GetAllRatings(results);
            Appraisers.AppraisersManager.PrintRatingsToConsole(ratings);
            Console.ReadLine();
        }
    }
}

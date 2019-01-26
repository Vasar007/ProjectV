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

        static void Main(string[] args)
        {
            var crawlerManager = new Crawlers.CrawlersManager
            {
                new Crawlers.TMDBCrawler()
            };
            var results = crawlerManager.GetAllData(Movies);
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

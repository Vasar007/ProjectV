using System;
using Newtonsoft.Json.Linq;

namespace ThingAppraiser
{
    class Program
    {
        static readonly string[] Movies = { "Allied", "Venom", "Sayonara no asa ni yakusoku no hana o kazaro" };

        static void Main(string[] args)
        {
            string[] names = { };
            if (args.Length == 1)
                names = Input.Input.GetNamesFromFile(args[0]);
            else
                names = Input.Input.GetNamesFromFile();
            
            var crawler = new Crawlers.TMDBCrawler();
            var results = crawler.GetData(names);

            foreach (var result in results)
            {
                Console.WriteLine(JToken.FromObject(result));
            }
            Console.ReadLine();
        }
    }
}

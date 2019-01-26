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

            var input = new input.Input();
            string[] names = { };
            try
            {
                names = input.ReadLines(args[0]);
            }
            catch
            {
                Console.WriteLine("Incorrect file name");
            }
            while (names.Length == 0)
            {
                Console.WriteLine("input other file name");
                try
                {
                    names = input.ReadLines(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Incorrect file name");
                }
                for(var i = 0; i < names.Length; ++i)
                {
                    Console.WriteLine(names[i]);
                }
            }

            foreach (var result in results)
            {
                Console.WriteLine(JToken.FromObject(result));
            }
            Console.ReadLine();
        }
    }
}

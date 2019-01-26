using System;
using Newtonsoft.Json.Linq;

namespace ThingAppraiser
{
    class Program
    {
        static readonly string[] Movies = { "Allied", "Venom", "Sayonara no asa ni yakusoku no hana o kazaro" };

        // Function for choosing input type
        private static string[] getNames(string[] args)
        {
            var input = new input.Input();
            string[] names = { };
            try
            {
                if(args.Length == 1)
                {
                    names = input.ReadLines(args[0]);
                }
                else
                {
                    names = input.ReadLines("scan_names.txt");
                }
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

            return names;
        }

        static void Main(string[] args)
        {
            var names = getNames(args);
            
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

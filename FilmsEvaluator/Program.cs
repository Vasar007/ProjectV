using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace FilmsEvaluator
{
    class Program
    {
        static readonly string[] Films = { "Allied", "Venom", "Sayonara no asa ni yakusoku no hana o kazaro" };

        static void Main(string[] args)
        {
            var crawler = new Crawlers.TMDBCrawler();
            var results = crawler.GetFilmsInfo(Films);
            foreach (var result in results)
            {
                Console.WriteLine(JToken.FromObject(result));
            }
            Console.ReadLine();
        }
    }
}

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ThingAppraiser.Core
{
    public class ConsoleMessageHandler : IMessageHandler
    {
        public string GetMessage()
        {
            return Console.ReadLine();
        }

        public void OutputMessage(string message)
        {
            Console.WriteLine(message);
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

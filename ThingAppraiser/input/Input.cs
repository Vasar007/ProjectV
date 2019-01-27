using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ThingAppraiser.Input
{
    public class Input
    {
        static readonly string DefaultFileName = "scan_names.txt";

        public static string[] GetNamesFromFile(string fileName = "")
        {
            string[] names = { };
            try
            {
                if(fileName != "")
                {
                    names = FileReader.ReadNames(fileName);
                }
                else
                {
                    names = FileReader.ReadNames(DefaultFileName);
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
                    names = FileReader.ReadNames(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Incorrect file name");
                }
            }

            return names;
        }
    }
}

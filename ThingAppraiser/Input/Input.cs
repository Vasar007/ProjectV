using System;

namespace ThingAppraiser.Input
{
    public class Input
    {
        private const string _defaultFileName = "scan_names.txt";

        public static string[] GetNamesFromFile(string fileName = "")
        {
            string[] names = { };
            try
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    names = FileReader.ReadNames(fileName);
                }
                else
                {
                    names = FileReader.ReadNames(_defaultFileName);
                }
            }
            catch
            {
                Console.WriteLine("Incorrect file name!");
            }

            while (names.Length == 0)
            {
                Console.WriteLine("Input other file name:");
                try
                {
                    names = FileReader.ReadNames(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Incorrect file name!");
                }
            }

            return names;
        }
    }
}

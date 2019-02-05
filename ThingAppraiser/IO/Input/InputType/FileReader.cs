using System.Collections.Generic;
using System.IO;

namespace ThingAppraiser.Input
{
    public static class FileReader
    {
        private static string[] ReadRawFile(string fileName)
        {
            var res = new List<string>();
            using (var reader = new StreamReader(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    res.Add(line.Trim('\r', '\n', ' '));
                }
                // Scanning name of product and removing special symbols.
            }
            return res.ToArray();
        }

        public static string[] ReadNames(string fileName)
        {
            string[] res = { };
            if (fileName.EndsWith(".txt"))
            {
                res = ReadRawFile(fileName);
            }
            return res;
        }
    }
}

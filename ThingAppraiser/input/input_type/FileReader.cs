using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ThingAppraiser.Input
{
    class FileReader
    {
        private static string[] ReadRawFile(string fileName)
        {
            List<string> res = new List<string>();
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
            if (fileName.Split('.').Last() == "txt")
            {
                res = ReadRawFile(fileName);
            }
            return res;
        }
    }
}

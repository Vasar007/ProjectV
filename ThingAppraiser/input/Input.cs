using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ThingAppraiser.input
{
    public class Input
    {
        public string[] ReadLines(string fileName)
        {
            List<string> res = new List<string>();
            using (var reader = new StreamReader(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    res.Add(line.Trim(new[] { '\r', '\n', ' ' }));
                }
                // Scanning name of product and removing special symbols.
            }
            return res.ToArray();
        }
    }
}

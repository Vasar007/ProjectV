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
                    // Do stuff with your line here, it will be called for each 
                    // line of text in your file.
                    res.Add(line.Trim(new[] { '\r', '\n', ' ' }));
                }
            }
            return res.ToArray();
        }
    }
}

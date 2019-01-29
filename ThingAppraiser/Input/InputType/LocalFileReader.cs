using System.Collections.Generic;
using System.IO;

namespace ThingAppraiser.Input
{
    public class LocalFileReader : Inputter
    {
        private List<string> ReadRawFile(string filename)
        {
            var result = new List<string>();
            using (var reader = new StreamReader(filename))
            {
                // Scanning name of things and removing special symbols.
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    result.Add(line.Trim('\r', '\n', ' '));
                }
            }
            return result;
        }

        public override List<string> ReadNames(string storageName)
        {
            var result = new List<string>();
            if (storageName.EndsWith(".txt"))
            {
                result = ReadRawFile(storageName);
            }
            return result;
        }
    }
}

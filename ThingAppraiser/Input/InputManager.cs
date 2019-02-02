using System;
using System.Collections.Generic;

namespace ThingAppraiser.Input
{
    public class InputManager
    {
        private const string _defaultFilename = "scan_names.txt";

        private Inputter _inputter;

        public InputManager(Inputter inputter)
        {
            _inputter = inputter;
        }

        private bool TryReadNames(ref List<string> result, string storageName)
        {
            try
            {
                result = _inputter.ReadNames(storageName);
            }
            catch
            {
                Console.WriteLine("Error! Cannot read file.");
                return false;
            }
            return true;
        }

        public List<string> GetNames(string storageName = "")
        {
            var result = new List<string>();
            if (!string.IsNullOrEmpty(storageName))
            {
                TryReadNames(ref result, storageName);
            }
            else
            {
                TryReadNames(ref result, _defaultFilename);
            }

            while (result.Count == 0)
            {
                Console.WriteLine("No Things were found. Enter other storage name:");
                TryReadNames(ref result, Console.ReadLine());
            }

            return result;
        }
    }
}

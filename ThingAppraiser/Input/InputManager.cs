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

        public List<string> GetNames(string storageName = "")
        {
            var result = new List<string>();
            try
            {
                if (!string.IsNullOrEmpty(storageName))
                {
                    result = _inputter.ReadNames(storageName);
                }
                else
                {
                    result = _inputter.ReadNames(_defaultFilename);
                }
            }
            catch
            {
                Console.WriteLine("Incorrect storage name!");
            }

            while (result.Count == 0)
            {
                Console.WriteLine("Input other storage name:");
                try
                {
                    result = _inputter.ReadNames(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Incorrect storage name!");
                }
            }

            return result;
        }
    }
}

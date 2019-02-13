using System;
using System.Collections.Generic;

namespace ThingAppraiser.IO.Input
{
    public class InputManager
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private const string _defaultFilename = "thing_names.txt";

        private IInputter _inputter;

        public InputManager(IInputter inputter)
        {
            _inputter = inputter;
        }

        private bool TryReadThingNames(ref List<string> result, string storageName)
        {
            try
            {
                result = _inputter.ReadThingNames(storageName);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Couldn't get access to the storage.");
                Console.WriteLine($"Couldn't get access to the storage. Error: {ex.Message}");
                return false;
            }
            return true;
        }

        public List<string> GetNames(string storageName = _defaultFilename)
        {
            var result = new List<string>();
            if (!string.IsNullOrEmpty(storageName))
            {
                TryReadThingNames(ref result, storageName);
            }
            else
            {
                TryReadThingNames(ref result, _defaultFilename);
            }

            while (result.Count == 0)
            {
                _logger.Warn("No Things were found.");
                Console.Write("No Things were found. Enter other storage name: ");
                TryReadThingNames(ref result, Console.ReadLine());
            }

            _logger.Info("Things were found and parsed.");
            return result;
        }
    }
}

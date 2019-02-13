using System.Collections.Generic;

namespace ThingAppraiser.IO.Output
{
    public class OutputManager
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private const string _defaultFilename = "apparaised_things.csv";

        private IOutputter _outputter;

        public OutputManager(IOutputter outputter)
        {
            _outputter = outputter;
        }

        public bool SaveResults(List<List<Data.ResultType>> results,
                                string storageName = _defaultFilename)
        {
            bool result = _outputter.SaveResults(results, storageName);
            if (result) _logger.Info($"Successfully saved results to {storageName}.");
            else _logger.Info($"Couldn't save results to {storageName}.");
            return result;
        }
    }
}

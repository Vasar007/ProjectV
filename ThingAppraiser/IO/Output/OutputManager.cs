using System.Collections.Generic;

namespace ThingAppraiser.IO.Output
{
    public class OutputManager
    {
        private const string _defaultFilename = "apparaised_things.csv";

        private IOutputter _outputter;

        public OutputManager(IOutputter outputter)
        {
            _outputter = outputter;
        }

        public bool SaveResults(List<List<Data.ResultType>> results,
                                string storageName = _defaultFilename)
        {
            return _outputter.SaveResults(results, storageName);
        }
    }
}

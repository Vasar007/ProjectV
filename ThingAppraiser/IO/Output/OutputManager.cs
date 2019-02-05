using System;
using System.Collections.Generic;
using System.Linq;

namespace ThingAppraiser.IO.Output
{
    public class OutputManager
    {
        private const string _defaultFilename = "apparaised_things.txt";

        private Outputter _outputter;

        public OutputManager(Outputter outputter)
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

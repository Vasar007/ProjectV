using System.Collections.Generic;
using ThingAppraiser.Data;

namespace ThingAppraiser.IO.Output.WebService
{
    public class OutputTransmitter : IOutputter, IOutputterBase, ITagable
    {
        private List<List<RatingDataContainer>> _transmittingResults;

        public string StorageName { get; private set; }

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = "OutputTransmitter";

        #endregion


        public OutputTransmitter()
        {
        }

        public List<List<RatingDataContainer>> GetResults()
        {
            if (_transmittingResults is null)
            {
                return new List<List<RatingDataContainer>>();
            }
            return _transmittingResults ;
        }

        #region IOutputter Implementation

        public bool SaveResults(List<List<RatingDataContainer>> results, string storageName)
        {
            StorageName = storageName;

            _transmittingResults = results;
            return true;
        }

        #endregion
    }
}

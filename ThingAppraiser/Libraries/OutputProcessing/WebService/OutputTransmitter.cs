using System.Collections.Generic;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.IO.Output.WebService
{
    public sealed class OutputTransmitter : IOutputter, IOutputterBase, ITagable
    {
        private List<List<RatingDataContainer>> _transmittingResults =
            new List<List<RatingDataContainer>>();

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(OutputTransmitter);

        #endregion

        public string StorageName { get; private set; } = string.Empty;


        public OutputTransmitter()
        {
        }

        public List<List<RatingDataContainer>> GetResults()
        {
            return _transmittingResults;
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

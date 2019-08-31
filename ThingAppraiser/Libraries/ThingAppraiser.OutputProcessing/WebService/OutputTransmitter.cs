using System.Collections.Generic;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.IO.Output.WebService
{
    public sealed class OutputTransmitter : IOutputter, IOutputterBase, ITagable
    {
        private IReadOnlyList<IReadOnlyList<RatingDataContainer>> _transmittingResults =
            new List<IReadOnlyList<RatingDataContainer>>();

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(OutputTransmitter);

        #endregion

        public string StorageName { get; private set; } = string.Empty;


        public OutputTransmitter()
        {
        }

        public IReadOnlyList<IReadOnlyList<RatingDataContainer>> GetResults()
        {
            return _transmittingResults;
        }

        #region IOutputter Implementation

        public bool SaveResults(IReadOnlyList<IReadOnlyList<RatingDataContainer>> results,
            string storageName)
        {
            StorageName = storageName;

            _transmittingResults = results;
            return true;
        }

        #endregion
    }
}

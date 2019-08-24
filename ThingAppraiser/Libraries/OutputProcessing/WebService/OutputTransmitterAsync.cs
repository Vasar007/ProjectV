using System.Collections.Generic;
using System.Threading.Tasks;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.IO.Output.WebService
{
    public sealed class OutputTransmitterAsync : IOutputterAsync, IOutputterBase, ITagable
    {
        private List<List<RatingDataContainer>> _transmittingResults =
            new List<List<RatingDataContainer>>();

        public string StorageName { get; private set; } = string.Empty;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(OutputTransmitter);

        #endregion


        public OutputTransmitterAsync()
        {
        }

        public List<List<RatingDataContainer>> GetResults()
        {
            return _transmittingResults ;
        }

        #region IOutputter Implementation

        public async Task<bool> SaveResults(List<List<RatingDataContainer>> results,
            string storageName)
        {
            StorageName = storageName;

            _transmittingResults = results;
            return await Task.FromResult(true);
        }

        #endregion
    }
}

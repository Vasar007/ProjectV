using System.Collections.Generic;
using System.Threading.Tasks;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.IO.Output.WebService
{
    public sealed class OutputTransmitterAsync : IOutputterAsync, IOutputterBase, ITagable
    {
        private IReadOnlyList<IReadOnlyList<RatingDataContainer>> _transmittingResults =
            new List<IReadOnlyList<RatingDataContainer>>();

        public string StorageName { get; private set; } = string.Empty;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(OutputTransmitter);

        #endregion


        public OutputTransmitterAsync()
        {
        }

        public IReadOnlyList<IReadOnlyList<RatingDataContainer>> GetResults()
        {
            return _transmittingResults ;
        }

        #region IOutputter Implementation

        public Task<bool> SaveResults(IReadOnlyList<IReadOnlyList<RatingDataContainer>> results,
            string storageName)
        {
            StorageName = storageName;

            _transmittingResults = results;
            return Task.FromResult(true);
        }

        #endregion
    }
}

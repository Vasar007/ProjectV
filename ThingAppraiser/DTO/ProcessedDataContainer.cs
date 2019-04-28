using System.Collections.Generic;

namespace ThingAppraiser.Data
{
    public class CProcessedDataContainer
    {
        private readonly List<CResultList> _processedData;

        public CRatingsStorage RatingsStorage { get; }


        public CProcessedDataContainer(List<CResultList> processedData,
            CRatingsStorage ratingsStorage)
        {
            _processedData = processedData.ThrowIfNull(nameof(processedData));
            RatingsStorage = ratingsStorage.ThrowIfNull(nameof(ratingsStorage));
        }

        public IReadOnlyList<CResultList> GetData()
        {
            return _processedData;
        }
    }
}

using System.Collections.Generic;

namespace ThingAppraiser.Data
{
    public class ProcessedDataContainer
    {
        private readonly List<ResultList> _processedData;

        public RatingsStorage RatingsStorage { get; }


        public ProcessedDataContainer(List<ResultList> processedData,
            RatingsStorage ratingsStorage)
        {
            _processedData = processedData.ThrowIfNull(nameof(processedData));
            RatingsStorage = ratingsStorage.ThrowIfNull(nameof(ratingsStorage));
        }

        public IReadOnlyList<ResultList> GetData()
        {
            return _processedData;
        }
    }
}

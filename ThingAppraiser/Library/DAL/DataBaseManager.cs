using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.DAL.Repositories;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.DAL
{
    public sealed class DataBaseManager
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<DataBaseManager>();

        private readonly IResultRepository _resultRepository;

        private readonly IRatingRepository _ratingRepository;

        public DataRepositoriesManager DataRepositoriesManager {get; } =
            new DataRepositoriesManager();


        public DataBaseManager(IResultRepository resultRepository, 
            IRatingRepository ratingRepository)
        {
            _resultRepository = resultRepository.ThrowIfNull(nameof(resultRepository));
            _ratingRepository = ratingRepository.ThrowIfNull(nameof(ratingRepository));
        }

        public List<List<BasicInfo>> GetResultsFromDb()
        {
            return DataRepositoriesManager.GetResultsFromDb();
        }

        public List<RawDataContainer> GetResultsFromDbWithAdditionalInfo()
        {
            return DataRepositoriesManager.GetResultsFromDbWithAdditionalInfo();
        }

        public List<List<RatingDataContainer>> GetRatingValuesFromDb(
            RatingsStorage ratingsStorage)
        {
            var results = new List<List<RatingDataContainer>>();

            List<Rating> ratings = _ratingRepository.GetAllData();
            foreach (Rating rating in ratings)
            {
                List<ThingIdWithRating> ratingsValue = _resultRepository.GetOrderedRatingsValue(
                    rating.RatingId
                );

                List<RatingDataContainer> dataHandlers = ratingsValue.Select(
                    thingIdWithRating => new RatingDataContainer(
                        DataRepositoriesManager.GetProperDataHandlerById(
                            thingIdWithRating.ThingId,
                            ratingsStorage.GetTypeByRatingId(rating.RatingId)
                        ),
                        thingIdWithRating.Rating
                    )
                ).ToList();
                results.Add(dataHandlers);
            }

            return results;
        }

        public void PutResultsToDb(List<List<BasicInfo>> results)
        {
            DataRepositoriesManager.PutResultsToDb(results);
        }

        public void PutRatingsToDb(ProcessedDataContainer ratings)
        {
            foreach (Rating rating in ratings.RatingsStorage.GetAllRatings())
            {
                if (!_ratingRepository.Contains(rating.RatingId))
                {
                    _ratingRepository.InsertItem(rating);
                }
            }

            foreach (ResultList datum in ratings.GetData())
            {
                // Skip empty collections of data.
                if (datum.IsNullOrEmpty()) continue;

                foreach (ResultInfo info in datum)
                {
                    _resultRepository.InsertItem(info);
                }
            }
        }

        public void DeleteData()
        {
            _logger.Info("Deletes all data in repositories.");
            DataRepositoriesManager.DeleteData();
        }

        public void DeleteResultAndRatings()
        {
            _logger.Info("Deletes result and ratings.");
            _resultRepository.DeleteAllData();
            _ratingRepository.DeleteAllData();
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Acolyte.Assertions;
using Acolyte.Collections;
using ProjectV.DataAccessLayer.Repositories;
using ProjectV.Logging;
using ProjectV.Models.Data;
using ProjectV.Models.Internal;

namespace ProjectV.DataAccessLayer
{
    public sealed class DatabaseManager
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<DatabaseManager>();

        private readonly IResultRepository _resultRepository;

        private readonly IRatingRepository _ratingRepository;

        public DataRepositoriesManager DataRepositoriesManager { get; } =
            new DataRepositoriesManager();


        public DatabaseManager(IResultRepository resultRepository, 
            IRatingRepository ratingRepository)
        {
            _resultRepository = resultRepository.ThrowIfNull(nameof(resultRepository));
            _ratingRepository = ratingRepository.ThrowIfNull(nameof(ratingRepository));
        }

        public IReadOnlyList<IReadOnlyList<BasicInfo>> GetResultsFromDb()
        {
            return DataRepositoriesManager.GetResultsFromDb();
        }

        public IReadOnlyList<RawDataContainer> GetResultsFromDbWithAdditionalInfo()
        {
            return DataRepositoriesManager.GetResultsFromDbWithAdditionalInfo();
        }

        public IReadOnlyList<IReadOnlyList<RatingDataContainer>> GetRatingValuesFromDb(
            RatingsStorage ratingsStorage)
        {
            var results = new List<IReadOnlyList<RatingDataContainer>>();

            IReadOnlyList<Rating> ratings = _ratingRepository.GetAllData();
            foreach (Rating rating in ratings)
            {
                IReadOnlyList<ThingIdWithRating> ratingsValue =
                    _resultRepository.GetOrderedRatingsValue(rating.RatingId);

                IReadOnlyList<RatingDataContainer> dataHandlers = ratingsValue.Select(
                    thingIdWithRating => new RatingDataContainer(
                        DataRepositoriesManager.GetProperDataHandlerById(
                            thingIdWithRating.ThingId,
                            ratingsStorage.GetTypeByRatingId(rating.RatingId)
                        ),
                        thingIdWithRating.Rating,
                        rating.RatingId
                    )
                ).ToList();
                results.Add(dataHandlers);
            }

            return results;
        }

        public void PutResultsToDb(IReadOnlyList<IReadOnlyList<BasicInfo>> results)
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

            foreach (IReadOnlyList<ResultInfo> datum in ratings.Data)
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

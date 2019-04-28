using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Communication;
using ThingAppraiser.DAL.Repositories;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DAL
{
    public sealed class CDataBaseManager
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CDataBaseManager>();

        private readonly IResultRepository _resultRepository;

        private readonly IRatingRepository _ratingRepository;

        public CDataRepositoriesManager DataRepositoriesManager {get; } =
            new CDataRepositoriesManager();


        public CDataBaseManager(IResultRepository resultRepository, 
            IRatingRepository ratingRepository)
        {
            _resultRepository = resultRepository.ThrowIfNull(nameof(resultRepository));
            _ratingRepository = ratingRepository.ThrowIfNull(nameof(ratingRepository));
        }

        public List<List<CBasicInfo>> GetResultsFromDB()
        {
            return DataRepositoriesManager.GetResultsFromDB();
        }

        public List<CRawDataContainer> GetResultsFromDBWithAdditionalInfo()
        {
            return DataRepositoriesManager.GetResultsFromDBWithAdditionalInfo();
        }

        public List<List<CRatingDataContainer>> GetRatingValuesFromDB(
            CRatingsStorage ratingsStorage)
        {
            var results = new List<List<CRatingDataContainer>>();

            List<CRating> ratings = _ratingRepository.GetAllData();
            foreach (CRating rating in ratings)
            {
                List<CThingIDWithRating> ratingsValue = _resultRepository.GetOrderedRatingsValue(
                    rating.RatingID
                );

                List<CRatingDataContainer> dataHandlers = ratingsValue.Select(
                    thingIDWithRating => new CRatingDataContainer(
                        DataRepositoriesManager.GetProperDataHandlerByID(
                            thingIDWithRating.ThingID,
                            ratingsStorage.GetTypeByRatingID(rating.RatingID)
                        ),
                        thingIDWithRating.Rating
                    )
                ).ToList();
                results.Add(dataHandlers);
            }

            return results;
        }

        public void PutResultsToDB(List<List<CBasicInfo>> results)
        {
            DataRepositoriesManager.PutResultsToDB(results);
        }

        public void PutRatingsToDB(CProcessedDataContainer ratings)
        {
            foreach (CRating rating in ratings.RatingsStorage.GetAllRatings())
            {
                if (!_ratingRepository.Contains(rating.RatingID))
                {
                    _ratingRepository.InsertItem(rating);
                }
            }

            foreach (CResultList datum in ratings.GetData())
            {
                // Skip empty collections of data.
                if (datum.IsNullOrEmpty()) continue;

                foreach (CResultInfo info in datum)
                {
                    _resultRepository.InsertItem(info);
                }
            }
        }

        public void DeleteData()
        {
            DataRepositoriesManager.DeleteData();
        }

        public void DeleteResultAndRatings()
        {
            _resultRepository.DeleteAllData();
            _ratingRepository.DeleteAllData();
        }
    }
}

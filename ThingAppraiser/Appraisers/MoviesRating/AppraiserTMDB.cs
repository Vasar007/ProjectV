using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Concrete appraiser for TMDB data.
    /// </summary>
    public class CAppraiserTMDB : CMoviesAppraiser
    {
        /// <inheritdoc />
        public override String Tag => "AppraiserTMDB";

        /// <inheritdoc />
        public override Type TypeID => typeof(CMovieTMDBInfo);

        /// <inheritdoc />
        public override String RatingName => "Rating based on popularity and votes";


        /// <summary>
        /// Default constructor.
        /// </summary>
        public CAppraiserTMDB()
        {
        }

        protected static Double CalculateRating(CMovieTMDBInfo entity, 
            CMinMaxDenominator voteCountMMD, CMinMaxDenominator voteAverageMMD, 
            CMinMaxDenominator popularityMMD)
        {
            Double baseValue = CalculateRating(entity, voteCountMMD, voteAverageMMD);
            Double popValue = (entity.Popularity - popularityMMD.MinValue) / 
                              popularityMMD.Denominator;

            return baseValue + popValue;
        }

        #region CMoviesAppraiser Overriden Methods

        /// <inheritdoc />
        /// <remarks>Consider popularity value in addition to average vote and vote count.</remarks>
        /// <exception cref="ArgumentException">
        /// <paramref name="rawDataContainer">rawDataContainer</paramref> contains instances of
        /// invalid type for this appraiser.
        /// </exception>
        public override CResultList GetRatings(CRawDataContainer rawDataContainer,
            Boolean outputResults)
        {
            CheckRatingID();

            var ratings = new CResultList();
            IReadOnlyList<CBasicInfo> rawData = rawDataContainer.GetData();
            if (rawData.IsNullOrEmpty()) return ratings;

            // Check if list have proper type.
            if (!rawData.All(e => e is CMovieTMDBInfo))
            {
                throw new ArgumentException(
                    $"Element type is invalid for appraiser with type {TypeID.FullName}"
                );
            }

            CMinMaxDenominator voteCountMMD = rawDataContainer.GetParameter("VoteCount");
            CMinMaxDenominator voteAverageMMD = rawDataContainer.GetParameter("VoteAverage");
            CMinMaxDenominator popularityMMD = rawDataContainer.GetParameter("Popularity");

            var converted = rawData.Select(e => (CMovieTMDBInfo) e);
            foreach (CMovieTMDBInfo entityInfo in converted)
            {
                Double ratingValue = CalculateRating(entityInfo, voteCountMMD, voteAverageMMD,
                                                     popularityMMD);

                var resultInfo = new CResultInfo(entityInfo.ThingID, ratingValue, RatingID);
                ratings.Add(resultInfo);

                if (outputResults)
                {
                    SGlobalMessageHandler.OutputMessage(resultInfo.ToString());
                }
            }

            ratings.Sort((x, y) => y.RatingValue.CompareTo(x.RatingValue));
            return ratings;
        }

        #endregion
    }
}

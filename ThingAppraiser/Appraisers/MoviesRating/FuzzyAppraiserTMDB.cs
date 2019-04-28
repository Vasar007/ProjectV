using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;
using ThingAppraiser.FuzzyLogicSystem;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Represents class which uses Fuzzy Logic Controller to calculate rating.
    /// </summary>
    public class CFuzzyAppraiserTMDB : CMoviesAppraiser
    {
        /// <summary>
        /// Fuzzy Controller which binds .NET interface with MATLAB module.
        /// </summary>
        private readonly IFuzzyController _fuzzyController = new FuzzyControllerIFuzzyController();

        /// <inheritdoc />
        public override String Tag => "FuzzyAppraiserTMDB";

        /// <inheritdoc />
        public override Type TypeID => typeof(CMovieTMDBInfo);

        /// <inheritdoc />
        public override String RatingName => "Fuzzy logic rating";


        /// <summary>
        /// Default constructor.
        /// </summary>
        public CFuzzyAppraiserTMDB()
        {
        }

        #region CMoviesAppraiser Overriden Methods

        /// <inheritdoc />
        /// <remarks>This appraiser uses MATLAB module to calculate ratings.</remarks>
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

            var converted = rawData.Select(e => (CMovieTMDBInfo) e);
            foreach (CMovieTMDBInfo entityInfo in converted)
            {
                Double ratingValue = _fuzzyController.CalculateRating(
                    entityInfo.VoteCount, entityInfo.VoteAverage, entityInfo.ReleaseDate.Year,
                    entityInfo.Popularity, entityInfo.Adult ? 1 : 0
                );

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

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
        public override Type TypeID => typeof(CMovieTMDBInfo);


        /// <summary>
        /// Default constructor.
        /// </summary>
        public CFuzzyAppraiserTMDB()
        {
        }

        #region CMoviesAppraiser Overriden Methods

        /// <summary>
        /// Calculates ratings based on average vote, vote count, year of release date, popularity
        /// and adult factor.
        /// </summary>
        /// <param name="entities">Entities to appraise.</param>
        /// <param name="outputResults">Flag to define need to output.</param>
        /// <returns>Collection of result object (data object with rating).</returns>
        /// <remarks>This appraiser uses MATLAB module to calculate ratings.</remarks>
        /// <exception cref="ArgumentException">
        /// <paramref name="entities">entities</paramref> contains instances of invalid type for
        /// this appraiser.
        /// </exception>
        public override CRating GetRatings(List<CBasicInfo> entities, Boolean outputResults)
        {
            var ratings = new CRating();
            if (entities.IsNullOrEmpty())
                return ratings;

            // Check if list have proper type.
            if (!entities.All(e => e is CMovieTMDBInfo))
            {
                throw new ArgumentException(
                    $"Element type is invalid for appraiser with type {TypeID.FullName}"
                );
            }

            var converted = entities.ConvertAll(e => (CMovieTMDBInfo) e);
            foreach (var entity in converted)
            {
                Single rating = _fuzzyController.CalculateRating(
                    entity.VoteCount, entity.VoteAverage, entity.ReleaseDate.Year,
                    entity.Popularity, entity.Adult ? 1 : 0
                );

                var resultInfo = new CResultInfo(entity, rating);
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

using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
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
        public override Type TypeID => typeof(CMovieTMDBInfo);


        /// <summary>
        /// Default constructor.
        /// </summary>
        public CAppraiserTMDB()
        {
        }

        #region CMoviesAppraiser Overriden Methods

        /// <inheritdoc />
        /// <remarks>Consider popularity value in addition to average vote and vote count.</remarks>
        /// <exception cref="ArgumentException">
        /// <paramref name="entities">entities</paramref> contains instances of invalid type for
        /// this appraiser.
        /// </exception>
        public override CRating GetRatings(List<CBasicInfo> entities, Boolean outputResults)
        {
            var ratings = new CRating();
            if (entities.IsNullOrEmpty()) return ratings;

            // Check if list have proper type.
            if (!entities.All(e => e is CMovieTMDBInfo))
            {
                throw new ArgumentException(
                    $"Element type is invalid for appraiser with type {TypeID.FullName}"
                );
            }

            var converted = entities.ConvertAll(e => (CMovieTMDBInfo) e);

            var normalizerVA = new CNormalizer<Single, CMovieTMDBInfo>(converted,
                                                                       c => c.VoteAverage);
            var normalizerVC = new CNormalizer<Int32, CMovieTMDBInfo>(converted, c => c.VoteCount);
            var normalizerPopularity = new CNormalizer<Single, CMovieTMDBInfo>(converted,
                                                                               c => c.Popularity);

            // Use additional property (Popularity).
            var enumerator = converted.ZipShortest(normalizerVA.Normalize(),
                                                   normalizerVC.Normalize(),
                                                   normalizerPopularity.Normalize(),
                                                   (t1, t2, t3, t4) => (t1, t2, t3, t4));
            foreach (var (entity, normValueVA, normValueVC, normValuePopularity) in enumerator)
            {
                var resultInfo = new CResultInfo(entity,
                                                 normValueVA + normValueVC + normValuePopularity);
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

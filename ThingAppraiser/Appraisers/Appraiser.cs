using System;
using System.Collections.Generic;
using MoreLinq;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Basic appraiser with default rating calculations.
    /// </summary>
    public abstract class CAppraiser
    {
        /// <summary>
        /// Defines which type of data objects this appraiser can process.
        /// </summary>
        public virtual Type TypeID => typeof(CBasicInfo);


        /// <summary>
        /// Default constructor.
        /// </summary>
        public CAppraiser()
        {
        }

        /// <summary>
        /// Makes prior analysis through normalizers and calculates ratings based on average vote 
        /// and vote count.
        /// </summary>
        /// <param name="entities">Entities to appraise.</param>
        /// <param name="outputResults">Flag to define need to output.</param>
        /// <returns>Collection of result object (data object with rating).</returns>
        /// <remarks>
        /// Entities collection must be unique because rating calculation errors can occur in such
        /// situations.
        /// </remarks>
        public virtual CRating GetRatings(List<CBasicInfo> entities, Boolean outputResults)
        {
            var ratings = new CRating();
            if (entities.IsNullOrEmpty()) return ratings;

            var normalizerVA = new CNormalizer<Single, CBasicInfo>(entities, e => e.VoteAverage);
            var normalizerVC = new CNormalizer<Int32, CBasicInfo>(entities, e => e.VoteCount);

            var enumerator = entities.ZipShortest(normalizerVA.Normalize(),
                                                  normalizerVC.Normalize(),
                                                  (t1, t2, t3) => (t1, t2, t3));

            foreach (var (entity, normValueVA, normValueVC) in enumerator)
            {
                var resultInfo = new CResultInfo(entity, normValueVA + normValueVC);
                ratings.Add(resultInfo);
                if (outputResults)
                {
                    SGlobalMessageHandler.OutputMessage(resultInfo.ToString());
                }
            }

            ratings.Sort((x, y) => y.RatingValue.CompareTo(x.RatingValue));
            return ratings;
        }
    }
}

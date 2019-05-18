using System;
using System.Collections.Generic;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Basic appraiser with default rating calculations. You should inherit this class if would 
    /// like to create your own appraiser with rating calculation.
    /// </summary>
    public abstract class Appraiser : AppraiserBase
    {
        #region ITagable Implementation

        /// <inheritdoc />
        public override string Tag { get; } = "Appraiser";

        #endregion

        /// <summary>
        /// Rating name which describes rating calculation.
        /// </summary>
        public override string RatingName { get; } = "Common rating";


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        protected Appraiser()
        {
        }

        protected static double CalculateRating(BasicInfo entity, MinMaxDenominator voteCountMMD,
            MinMaxDenominator voteAverageMMD)
        {
            double vcValue = (entity.VoteCount - voteCountMMD.MinValue) / 
                             voteCountMMD.Denominator;
            double vaValue = (entity.VoteAverage - voteAverageMMD.MinValue) / 
                             voteAverageMMD.Denominator;

            return vcValue + vaValue;
        }

        /// <summary>
        /// Makes prior analysis through normalizers and calculates ratings based on average vote 
        /// and vote count.
        /// </summary>
        /// <param name="rawDataContainer">Entities to appraise with additional parameters.</param>
        /// <param name="outputResults">Flag to define need to output.</param>
        /// <returns>Collection of result object (data object with rating).</returns>
        /// <remarks>
        /// Entities collection must be unique because rating calculation errors can occur in such
        /// situations.
        /// </remarks>
        public virtual ResultList GetRatings(RawDataContainer rawDataContainer, bool outputResults)
        {
            CheckRatingId();

            var ratings = new ResultList();
            IReadOnlyList<BasicInfo> rawData = rawDataContainer.GetData();
            if (rawData.IsNullOrEmpty()) return ratings;

            MinMaxDenominator voteCountMMD = rawDataContainer.GetParameter("VoteCount");
            MinMaxDenominator voteAverageMMD = rawDataContainer.GetParameter("VoteAverage");

            foreach (BasicInfo entityInfo in rawData)
            {
                double ratingValue = CalculateRating(entityInfo, voteCountMMD, voteAverageMMD);

                var resultInfo = new ResultInfo(entityInfo.ThingId, ratingValue, RatingId);
                ratings.Add(resultInfo);

                if (outputResults)
                {
                    GlobalMessageHandler.OutputMessage(resultInfo.ToString());
                }
            }

            ratings.Sort((x, y) => y.RatingValue.CompareTo(x.RatingValue));
            return ratings;
        }

        /// <summary>
        /// Checks that class was registered rating and has proper rating ID.
        /// </summary>
        protected void CheckRatingId()
        {
            if (RatingId == Guid.Empty)
            {
                throw new InvalidOperationException(
                    "Rating ID has no value."
                );
            }
        }
    }
}

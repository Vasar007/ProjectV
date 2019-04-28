using System;
using System.Collections.Generic;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Basic appraiser with default rating calculations.
    /// </summary>
    public abstract class CAppraiser : ITagable, ITypeID
    {
        #region ITagable Implementation

        /// <inheritdoc />
        public virtual String Tag => "Appraiser";

        #endregion

        #region ITypeID Implementation

        /// <summary>
        /// Defines which type of data objects this appraiser can process.
        /// </summary>
        public virtual Type TypeID => typeof(CBasicInfo);

        #endregion

        /// <summary>
        /// Rating name which describes rating calculation.
        /// </summary>
        public virtual String RatingName => "Common rating";

        /// <summary>
        /// Specify rating ID for result.
        /// </summary>
        public Guid RatingID { get; set; }


        /// <summary>
        /// Default constructor.
        /// </summary>
        public CAppraiser()
        {
        }

        protected static Double CalculateRating(CBasicInfo entity, CMinMaxDenominator voteCountMMD,
            CMinMaxDenominator voteAverageMMD)
        {
            Double vcValue = (entity.VoteCount - voteCountMMD.MinValue) / 
                             voteCountMMD.Denominator;
            Double vaValue = (entity.VoteAverage - voteAverageMMD.MinValue) / 
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
        public virtual CResultList GetRatings(CRawDataContainer rawDataContainer,
            Boolean outputResults)
        {
            CheckRatingID();

            var ratings = new CResultList();
            IReadOnlyList<CBasicInfo> rawData = rawDataContainer.GetData();
            if (rawData.IsNullOrEmpty()) return ratings;

            CMinMaxDenominator voteCountMMD = rawDataContainer.GetParameter("VoteCount");
            CMinMaxDenominator voteAverageMMD = rawDataContainer.GetParameter("VoteAverage");

            foreach (CBasicInfo entityInfo in rawData)
            {
                Double ratingValue = CalculateRating(entityInfo, voteCountMMD, voteAverageMMD);

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

        /// <summary>
        /// Checks that class was registered rating and has proper rating ID.
        /// </summary>
        protected void CheckRatingID()
        {
            if (RatingID == Guid.Empty)
            {
                throw new InvalidOperationException(
                    "Rating ID has no value."
                );
            }
        }
    }
}

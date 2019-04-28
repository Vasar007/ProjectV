using System;

namespace ThingAppraiser.Data
{
    [Serializable]
    public class CRating
    {
        public Guid RatingID { get; }

        public String RatingName { get; }


        public CRating(Guid ratingID, String ratingName)
        {
            RatingID = ratingID;
            RatingName = ratingName;
        }

        #region Object Overridden Methods

        /// <inheritdoc />
        public override Boolean Equals(Object obj)
        {
            if (obj is null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is CRating rating)) return false;

            return RatingID == rating.RatingID &&
                   RatingName.IsEqualWithInvariantCulture(rating.RatingName);
        }

        /// <inheritdoc />
        public override Int32 GetHashCode()
        {
            return RatingID.GetHashCode();
        }

        #endregion
    }
}

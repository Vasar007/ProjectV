using System;

namespace ThingAppraiser.Models.Internal
{
    public sealed class Rating
    {
        public Guid RatingId { get; }

        public string RatingName { get; }


        public Rating(Guid ratingId, string ratingName)
        {
            RatingId = ratingId;
            RatingName = ratingName;
        }

        #region Object Overridden Methods

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is Rating rating)) return false;

            return RatingId == rating.RatingId &&
                   string.Equals(RatingName, rating.RatingName, StringComparison.InvariantCulture);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return RatingId.GetHashCode();
        }

        #endregion
    }
}

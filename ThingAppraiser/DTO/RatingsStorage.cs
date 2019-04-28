using System;
using System.Collections.Generic;
using System.Linq;

namespace ThingAppraiser.Data
{
    public sealed class CRatingsStorage
    {
        private readonly Dictionary<CRating, Type> _ratingsHelper = new Dictionary<CRating, Type>();


        public CRatingsStorage()
        {
        }

        public Guid Register(Type dataHandlerType, String ratingName)
        {
            dataHandlerType.ThrowIfNull(nameof(dataHandlerType));
            if (!_ratingsHelper.ContainsValue(dataHandlerType))
            {
                var rating = new CRating(Guid.NewGuid(), ratingName);
                _ratingsHelper.Add(rating, dataHandlerType);
                return rating.RatingID;
            }

            return _ratingsHelper.FirstOrDefault(x => x.Value == dataHandlerType).Key.RatingID;
        }

        public Boolean Deregister(Guid ratingID)
        {
            CRating rating = _ratingsHelper.Keys.First(x => x.RatingID == ratingID);
            return _ratingsHelper.Remove(rating);
        }

        public CRating GetRatingByID(Guid ratingID)
        {
            return _ratingsHelper.Keys.First(x => x.RatingID == ratingID);
        }

        public List<CRating> GetAllRatings()
        {
            return _ratingsHelper.Keys.ToList();
        }

        public Type GetTypeByRatingID(Guid ratingID)
        {
            CRating rating = GetRatingByID(ratingID);
            if (!_ratingsHelper.TryGetValue(rating, out Type type))
            {
                throw new ArgumentException(
                    $"Rating with specified ID {ratingID} was not registered.", nameof(ratingID)
                );
            }
            return type;
        }
    }
}

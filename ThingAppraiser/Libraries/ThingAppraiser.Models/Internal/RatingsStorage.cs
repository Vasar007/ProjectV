using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.Models.Internal
{
    public sealed class RatingsStorage
    {
        private readonly Dictionary<Rating, Type> _ratingsHelper = new Dictionary<Rating, Type>();


        public RatingsStorage()
        {
        }

        public Guid Register(Type dataHandlerType, string ratingName)
        {
            dataHandlerType.ThrowIfNull(nameof(dataHandlerType));
            if (!_ratingsHelper.ContainsValue(dataHandlerType))
            {
                var rating = new Rating(Guid.NewGuid(), ratingName);
                _ratingsHelper.Add(rating, dataHandlerType);
                return rating.RatingId;
            }

            return _ratingsHelper.First(x => x.Value == dataHandlerType).Key.RatingId;
        }

        public bool Deregister(Guid ratingId)
        {
            Rating rating = _ratingsHelper.Keys.First(x => x.RatingId == ratingId);
            return _ratingsHelper.Remove(rating);
        }

        public Rating GetRatingById(Guid ratingId)
        {
            return _ratingsHelper.Keys.First(x => x.RatingId == ratingId);
        }

        public IReadOnlyList<Rating> GetAllRatings()
        {
            return _ratingsHelper.Keys.ToList();
        }

        public Type GetTypeByRatingId(Guid ratingId)
        {
            Rating rating = GetRatingById(ratingId);
            if (!_ratingsHelper.TryGetValue(rating, out Type type))
            {
                throw new ArgumentException(
                    $"Rating with specified ID {ratingId.ToString()} was not registered.",
                    nameof(ratingId)
                );
            }
            return type;
        }
    }
}

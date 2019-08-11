using System;

namespace ThingAppraiser.Models.Internal
{
    public class ThingIdWithRating
    {
        public int ThingId { get; }

        public double Rating { get; }


        public ThingIdWithRating(int id, double rating)
        {
            ThingId = id;
            Rating = rating;
        }
    }
}

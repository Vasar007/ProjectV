using System;

namespace ThingAppraiser.Data
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

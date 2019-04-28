using System;

namespace ThingAppraiser.Data
{
    [Serializable]
    public class CThingIDWithRating
    {
        public Int32 ThingID { get; }

        public Double Rating { get; }


        public CThingIDWithRating(Int32 id, Double rating)
        {
            ThingID = id;
            Rating = rating;
        }
    }
}

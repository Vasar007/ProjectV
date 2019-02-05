using System;

namespace ThingAppraiser.Data
{
    [Serializable()]
    public class ResultType
    {
        public DataHandler DataHandler { get; }
        public float RatingValue { get; }

        public ResultType(DataHandler dataHandler, float ratingValue)
        {
            DataHandler = dataHandler;
            RatingValue = ratingValue;
        }

        public void Deconstruct(out DataHandler dataHandler, out float ratingValue)
        {
            dataHandler = DataHandler;
            ratingValue = RatingValue;
        }
    }
}

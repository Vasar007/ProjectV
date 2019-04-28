using System;

namespace ThingAppraiser.Data
{
    public class CMinMaxDenominator
    {
        public Double MinValue { get; }

        public Double MaxValue { get; }

        public Double Denominator { get; }


        public CMinMaxDenominator(Double minValue, Double maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Denominator = maxValue - minValue;
        }
    }
}

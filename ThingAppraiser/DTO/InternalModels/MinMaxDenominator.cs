namespace ThingAppraiser.Data
{
    /// <summary>
    /// Represents pair of three numbers: minimum, maximum and their subtraction. Used for data 
    /// processing.
    /// </summary>
    public class MinMaxDenominator
    {
        /// <summary>
        /// Minimum value.
        /// </summary>
        public double MinValue { get; }

        /// <summary>
        /// Maximum value.
        /// </summary>
        public double MaxValue { get; }

        /// <summary>
        /// Subtraction of the minimum and maximum values.
        /// </summary>
        public double Denominator { get; }



        /// <summary>
        /// Initializes instance with passed numbers.
        /// </summary>
        /// <param name="minValue">Minimum value.</param>
        /// <param name="maxValue">Maximum value.</param>
        /// <remarks>
        /// Class doesn't contains any tools to control correct values for fields (i.e. class does 
        /// NOT check that <paramref name="minValue" /> &lt; <paramref name="maxValue" />).
        /// </remarks>
        public MinMaxDenominator(double minValue, double maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Denominator = maxValue - minValue;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Exceptions;

namespace ThingAppraiser.Models.Internal
{
    /// <summary>
    /// Represents pair of three numbers: minimum, maximum and their subtraction. Used for data 
    /// processing.
    /// </summary>
    public sealed class MinMaxDenominator
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
        /// If subtraction of <paramref name="minValue" /> and <paramref name="maxValue" /> is
        /// equal to zero, value of denominator would be set to one.
        /// </remarks>
        public MinMaxDenominator(double minValue, double maxValue)
        {
            if (minValue > maxValue)
            {
                throw new MultipleArgumentException(
                    "Min value is greater than max value.",
                    nameof(minValue), nameof(maxValue)
                );
            }

            MinValue = minValue;
            MaxValue = maxValue;
            Denominator = maxValue - minValue;

            if (Denominator == 0)
            {
                Denominator = 1;
            }
        }

        /// <summary>
        /// Creates <see cref="MinMaxDenominator" /> for the specified collectiuon.
        /// </summary>
        /// <typeparam name="T">The type of collection items.</typeparam>
        /// <param name="items">The collection to get min, max and denominator from.</param>
        /// <param name="selector">A function to extract value from every item.</param>
        /// <returns>Object with min, max and denominator values of specified collection.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items" /> is <c>null</c> -or-
        /// <paramref name="selector" /> is <c>null</c> .
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="items" /> presents empty collection.
        /// </exception>
        public static MinMaxDenominator CreateForCollection<T>(
            IReadOnlyCollection<T> items, Func<T, double> selector)
        {
            items.ThrowIfNullOrEmpty(nameof(items));
            selector.ThrowIfNull(nameof(selector));

            double maxValue = selector(items.First());
            double minValue = maxValue;

            foreach (T entity in items)
            {
                var value = selector(entity);

                if (value > maxValue)
                {
                    maxValue = value;
                }
                else if (value < minValue)
                {
                    minValue = value;
                }
            }

            return new MinMaxDenominator(minValue, maxValue);
        }
    }
}

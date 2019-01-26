using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MiscUtil;

namespace ThingAppraiser.Appraisers
{
    public class Normalizer<T, TL> where TL : Data.DataHandler 
    {
        private class MinMaxValues
        {
            public T maxValue;
            public T minValue;
            public T denominator;
        }

        private readonly List<TL> _entities;
        private readonly Func<TL, T> _property;

        public Normalizer(List<TL> entities, Expression<Func<TL, T>> property)
        {
            _entities = entities;

            // Check if we have valid property method.
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            if (propertyInfo is null)
            {
                throw new ArgumentException(
                    "The lambda expression 'property' should point to a valid Property."
                );
            }
            _property = property.Compile();
        }

        private MinMaxValues CalculateMinMax()
        {
            var maxValue = _entities.Max(e => _property(e));
            var minValue = _entities.Min(e => _property(e));
            // If T does not provide operator- an exception will be thrown.
            var denominator = Operator<T>.Subtract(maxValue, minValue);

            return new MinMaxValues()
            {
                maxValue = maxValue,
                minValue = minValue,
                denominator = denominator
            };
        }

        public IEnumerable<T> Normalize()
        {
            var minMaxValue = CalculateMinMax();
            foreach (var entity in _entities)
                // If T does not provide operator- and operator/ an exception will be thrown.
                yield return Operator<T>.Divide(
                    Operator<T>.Subtract(_property(entity), minMaxValue.minValue),
                    minMaxValue.denominator
                );
        }
    }
}

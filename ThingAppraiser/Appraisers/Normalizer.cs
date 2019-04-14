using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MiscUtil;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Encapsulates normalization and optimizes rating calculations.
    /// </summary>
    /// <typeparam name="T">Type of the property.</typeparam>
    /// <typeparam name="TL">Type of the data object.</typeparam>
    /// <remarks>
    /// ATTENTION! There are 2 contracts if you want use this class:
    /// 1) T should provide operator/, operator&lt; (less), operator&gt; (great) and 
    /// operator-. If this contract is broken the exception
    /// <exception cref="InvalidOperationException" /> will be thrown.
    /// 2) List of entities should contain at least one element. Else exception 
    /// <exception cref="InvalidOperationException" /> will be thrown.
    /// </remarks>
    public class CNormalizer<T, TL> where TL : CBasicInfo
    {
        private class CMinMaxDenominator
        {
            public T maxValue;
            public T minValue;
            public T denominator;
        }


        private readonly List<TL> _entities;

        private readonly Func<TL, T> _property;


        public CNormalizer(List<TL> entities, Expression<Func<TL, T>> property)
        {
            _entities = entities;

            // Check if we have valid property method.
            var propertyInfo = ((MemberExpression) property.Body).Member as PropertyInfo;
            if (propertyInfo is null)
            {
                throw new ArgumentException(
                    "The lambda expression 'property' should point to a valid Property."
                );
            }
            _property = property.Compile();
        }

        public IEnumerable<T> Normalize()
        {
            CMinMaxDenominator minMaxValue = CalculateCommonCharacteristics();
            foreach (TL entity in _entities)
                // If T does not provide operator- and operator/ an exception will be thrown.
                yield return Operator<T>.Divide(
                    Operator<T>.Subtract(_property(entity), minMaxValue.minValue),
                    minMaxValue.denominator
                );
        }

        private CMinMaxDenominator CalculateCommonCharacteristics()
        {
            T maxValue = _property(_entities.First());
            T minValue = maxValue;

            foreach (TL entity in _entities)
            {
                T value = _property(entity);

                // If T does not provide operator> and operator< an exception will be thrown.
                if (Operator<T>.GreaterThan(value, maxValue))
                {
                    maxValue = value;
                }
                else if (Operator<T>.LessThan(value, minValue))
                {
                    minValue = value;
                }
            }

            // If T does not provide operator- an exception will be thrown.
            T denominator = Operator<T>.Subtract(maxValue, minValue);

            return new CMinMaxDenominator
            {
                maxValue = maxValue,
                minValue = minValue,
                denominator = denominator
            };
        }
    }
}

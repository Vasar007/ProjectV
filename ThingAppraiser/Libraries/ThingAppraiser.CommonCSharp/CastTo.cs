using System;
using System.Linq.Expressions;
using Acolyte.Assertions;

namespace ThingAppraiser
{
    /// <summary>
    /// Class to cast to type <typeparamref name="TTarget"/>.
    /// </summary>
    /// <typeparam name="TTarget">Target type.</typeparam>
    /// <remarks>
    /// Original source: <see href="https://stackoverflow.com/a/23391746/8581036" />
    /// </remarks>
    public static class CastTo<TTarget>
    {
        private static class Cache<TSourceInternal>
        {
            public static readonly Func<TSourceInternal, TTarget> Caster = Get();

            private static Func<TSourceInternal, TTarget> Get()
            {
                var parameter = Expression.Parameter(typeof(TSourceInternal));
                var conversionBody = Expression.ConvertChecked(parameter, typeof(TTarget));

                return Expression
                    .Lambda<Func<TSourceInternal, TTarget>>(conversionBody, parameter)
                    .Compile();
            }
        }

        /// <summary>
        /// Casts <typeparamref name="TSource"/> to <typeparamref name="TTarget"/>.
        /// This does not cause boxing for value types.
        /// Useful in generic methods.
        /// </summary>
        /// <typeparam name="TSource">Source type to cast from. Usually a generic type.</typeparam>
        /// <returns>Casted value.</returns>
        public static TTarget From<TSource>(TSource source)
        {
            source.ThrowIfNullValue(nameof(source), assertOnPureValueTypes: false);

            return Cache<TSource>.Caster(source);
        }
    }
}

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows.Data;
using Acolyte.Assertions;

namespace ProjectV.DesktopApp.Domain.Validation
{
    internal static class BindingExpressionExtensions
    {
        public static T Evaluate<T>(this BindingExpression bindingExpression)
        {
            bindingExpression.ThrowIfNull(nameof(bindingExpression));

            object? resolvedSource = bindingExpression.ResolvedSource;
            if (resolvedSource is null)
            {
                throw new ArgumentException(
                    "Failed to evaluate binding expression. Source is not resolved.",
                    nameof(bindingExpression)
                );
            }

            Type resolvedType = resolvedSource.GetType();

            PropertyInfo? prop = resolvedType.GetProperty(
                bindingExpression.ResolvedSourcePropertyName
            );

            if (prop is null)
            {
                throw new ArgumentException(
                    "Failed to evaluate binding expression.", nameof(bindingExpression)
                );
            }

            return CastTo<T>.From(prop.GetValue(bindingExpression.ResolvedSource));
        }

        public static bool TryEvaluate<T>(this BindingExpression bindingExpression,
            [NotNullWhen(true)] out T result)
        {
            bindingExpression.ThrowIfNull(nameof(bindingExpression));

            object? resolvedSource = bindingExpression.ResolvedSource;
            if (resolvedSource is null)
            {
#pragma warning disable CS8601 // Possible null reference assignment.
                result = default;
#pragma warning restore CS8601 // Possible null reference assignment.
                return false;
            }

            Type resolvedType = resolvedSource.GetType();

            PropertyInfo? prop = resolvedType.GetProperty(
                bindingExpression.ResolvedSourcePropertyName
            );

            if (prop is not null &&
                prop.GetValue(bindingExpression.ResolvedSource) is T value &&
                value is not null)
            {
                result = value;
                return true;
            }

#pragma warning disable CS8601 // Possible null reference assignment.
            result = default;
#pragma warning restore CS8601 // Possible null reference assignment.
            return false;
        }

        [return: MaybeNull]
        public static T EvaluateOrDefault<T>(this BindingExpression bindingExpression,
            [AllowNull] T defaultValue)
        {
            if (TryEvaluate(bindingExpression, out T result)) return result;

            return defaultValue;
        }
    }
}

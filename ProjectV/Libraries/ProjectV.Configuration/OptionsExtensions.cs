using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Acolyte.Assertions;
using Microsoft.Extensions.Options;

namespace ProjectV.Configuration
{
    public static class OptionsExtensions
    {
        [return: NotNull]
        public static TOptions GetCheckedValue<TOptions>(this IOptions<TOptions>? options,
            [CallerArgumentExpression("options")] string paramName = "")
            where TOptions : class, IOptions
        {
            var value = options.ThrowIfNull(paramName)
                .Value.ThrowIfNull(paramName);

            value.Validate();

            return value;
        }
    }
}

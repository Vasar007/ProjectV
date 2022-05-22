using System.Runtime.CompilerServices;
using Acolyte.Assertions;
using Microsoft.Extensions.Options;

namespace ProjectV.Configuration
{
    public static class OptionsExtensions
    {
        public static TOptions GetCheckedValue<TOptions>(this IOptions<TOptions> options,
            [CallerArgumentExpression("options")] string paramName = "")
            where TOptions : class
        {
            options.ThrowIfNull(paramName);

            return options.Value.ThrowIfNull(paramName);
        }
    }
}

using System.Diagnostics.CodeAnalysis;
using Acolyte.Assertions;
using Microsoft.Extensions.Configuration;

namespace ProjectV.CommonWebApi.Service.Extensions
{
    public static class ConfigurationSectionExtensions
    {
        public static T GetChecked<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
            this IConfiguration configuration)
        {
            var configOption = configuration.Get<T>();
            return configOption.ThrowIfNullValue(nameof(configuration));
        }
    }
}

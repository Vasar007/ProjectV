using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;

namespace ProjectV.Configuration
{
    public static partial class ConfigOptions
    {
        private static readonly Lazy<IConfigurationRoot> Root =
            new Lazy<IConfigurationRoot>(LoadOptions);

        public static string ConfigFilename { get; } = "config.json";

        public static string AlternativeOptionsPath { get; } =
            Path.Combine("/etc", "ProjectV", ConfigFilename);

        public static ApiOptions Api => GetOptions<ApiOptions>();

        public static ProjectVServiceOptions ProjectVService =>
            GetOptions<ProjectVServiceOptions>();


        public static T GetOptions<T>()
            where T : IOptions, new()
        {
            T section = Root.Value.GetSection(typeof(T).Name).Get<T>();

            if (section == null) return new T();

            return section;
        }

        private static IConfigurationRoot LoadOptions()
        {
            var configurationBuilder = new ConfigurationBuilder();

            string configPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Path.Combine(Directory.GetCurrentDirectory(), ConfigFilename)
                : AlternativeOptionsPath;

            configurationBuilder.AddJsonFile(configPath, optional: true, reloadOnChange: true);
            return configurationBuilder.Build();
        }
    }
}

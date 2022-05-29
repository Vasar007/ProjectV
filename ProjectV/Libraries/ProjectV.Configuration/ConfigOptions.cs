﻿using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using ProjectV.Options;
using ProjectV.Json;
using ProjectV.Configuration.Options;

namespace ProjectV.Configuration
{
    public static partial class ConfigOptions
    {
        private static readonly Lazy<IConfigurationRoot> LazyRoot = new(LoadOptions);
        private static IConfigurationRoot Root => LazyRoot.Value;

        public static string DefaultOptionsPath => PredefinedPaths.DefaultOptionsPath;

        #region Options

        public static ApiKeysOptions ApiKeys => GetOptions<ApiKeysOptions>();

        public static ProjectVServiceOptions ProjectVService => GetOptions<ProjectVServiceOptions>();

        public static UserServiceOptions UserService => GetOptions<UserServiceOptions>();

        #endregion


        public static TOptions? FindOptions<TOptions>()
             where TOptions : class, IOptions, new()
        {
            IConfigurationSection section = GetConfigurationSection<TOptions>();
            return section.Get<TOptions>();
        }

        public static TOptions GetOptions<TOptions>()
            where TOptions : class, IOptions, new()
        {
            TOptions? options = FindOptions<TOptions>();

            // Sometimes options can be null because configuration data is reloading.
            if (options is null) return new TOptions();

            return options;
        }

        public static void SetOptions<TOptions>(TOptions? options)
        where TOptions : class, IOptions, new()
        {
            if (options is null) return;

            IConfigurationSection section = GetConfigurationSection<TOptions>();

            string output = JsonConvert.SerializeObject(
                options, JsonHelper.DefaultSerializerSettings
            );
            section.Value = output;
        }

        public static IChangeToken GetReloadToken()
        {
            return Root.GetReloadToken();
        }

        public static IChangeToken GetReloadToken<TOptions>()
            where TOptions : class, IOptions, new()
        {
            IConfigurationSection section = GetConfigurationSection<TOptions>();
            return section.GetReloadToken();
        }

        private static IConfigurationSection GetConfigurationSection<TOptions>()
            where TOptions : class, IOptions, new()
        {
            return Root.GetSection(typeof(TOptions).Name);
        }

        private static IConfigurationRoot LoadOptions()
        {
            var configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.AddJsonFile(
                path: DefaultOptionsPath,
                optional: true,
                reloadOnChange: true
            );

            return configurationBuilder.Build();
        }
    }
}

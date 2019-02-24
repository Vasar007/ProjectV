using System;
using System.Configuration;

namespace ThingAppraiser.Core
{
    /// <summary>
    /// Defines methods to interact with app config file and allows to read values from it.
    /// </summary>
    /// <remarks>Uses default app config file from .NET environment.</remarks>
    public static class ConfigParser
    {
        #region Public Static Methods

        /// <summary>
        /// Default method to get value from app config file as string.
        /// </summary>
        /// <param name="key">Name of the key to read.</param>
        /// <returns>Value of the key.</returns>
        public static string GetValueByParameterKey(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// Get value from app config as string and convert it to type T.
        /// </summary>
        /// <remarks>Method doesn't catch any possible exceptions.</remarks>
        /// <typeparam name="T">Target type to convert value.</typeparam>
        /// <param name="key">Name of the key to read.</param>
        /// <returns>Value of the key which is converted to T.</returns>
        public static T GetValueByParameterKey<T>(string key) where T : IConvertible
        {
            var stringValue = GetValueByParameterKey(key);
            return (T) Convert.ChangeType(stringValue, typeof(T));
        }

        #endregion
    }
}

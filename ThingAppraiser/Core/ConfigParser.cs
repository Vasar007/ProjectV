using System;
using System.Configuration;

namespace ThingAppraiser.Core
{
    /// <summary>
    /// Defines methods to interact with app config file and allows to read values from it.
    /// </summary>
    /// <remarks>Uses default app config file from .NET environment.</remarks>
    public static class SConfigParser
    {
        /// <summary>
        /// Default method to get value from app config file as string.
        /// </summary>
        /// <param name="key">Name of the key to read.</param>
        /// <returns>Value of the key.</returns>
        public static String GetValueByParameterKey(String key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// Gets value from app config as string and convert it to type T.
        /// </summary>
        /// <typeparam name="T">Target type to convert value.</typeparam>
        /// <param name="key">Name of the key to read.</param>
        /// <returns>Value of the key which is converted to T.</returns>
        /// <remarks>Method doesn't catch any possible exceptions.</remarks>
        public static T GetValueByParameterKey<T>(String key) where T : IConvertible
        {
            String stringValue = GetValueByParameterKey(key);
            return (T) Convert.ChangeType(stringValue, typeof(T));
        }
    }
}

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
        /// <summary>
        /// Default method to get value from app config file as string.
        /// </summary>
        /// <param name="key">Name of the key to read.</param>
        /// <returns>Value of the key.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="key" /> isn't contained in the dictionary -or-
        /// <paramref name="key" /> is <c>null</c> or presents empty string.
        /// </exception>
        public static string GetValueByParameterKey(string key)
        {
            key.ThrowIfNullOrEmpty(nameof(key));
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// Gets value from app config as string and convert it to type T.
        /// </summary>
        /// <typeparam name="T">Target type to convert value.</typeparam>
        /// <param name="key">Name of the key to read.</param>
        /// <returns>Value of the key which is converted to T.</returns>
        /// <remarks>Method doesn't catch any possible exceptions.</remarks>
        /// <exception cref="ArgumentException">
        /// <paramref name="key" /> isn't contained in the dictionary.
        /// </exception>
        ///  <exception cref="InvalidCastException">
        /// This conversion is not supported. -or- value is null and conversionType is a
        /// value type. -or- value does not implement the System.IConvertible interface.
        /// </exception>
        /// <exception cref="FormatException">
        /// Value is not in a format recognized by conversionType.
        /// </exception>
        /// <exception cref="OverflowException">
        /// Value represents a number that is out of the range of conversionType.
        /// </exception>
        /// <exception cref="ArgumentNullException">ConversionType is null.</exception>
        public static T GetValueByParameterKey<T>(string key)
            where T : IConvertible
        {
            string stringValue = GetValueByParameterKey(key);
            return (T) Convert.ChangeType(stringValue, typeof(T));
        }
    }
}

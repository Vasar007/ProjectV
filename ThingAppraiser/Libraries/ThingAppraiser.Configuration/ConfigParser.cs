using System;
using System.Configuration;

namespace ThingAppraiser.Configuration
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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="key" /> isn't contained in the config dictionary. -or-
        /// <paramref name="key" /> presents empty string.
        /// </exception>
        /// <exception cref="ConfigurationErrorsException">
        /// Could not retrieve a <see cref="System.Collections.Specialized.NameValueCollection" /> 
        /// with the application settings data.
        /// </exception>
        public static string GetValueByParameterKey(string key)
        {
            key.ThrowIfNullOrEmpty(nameof(key));
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// Gets value from app config as string and convert it to type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">Target type to convert value.</typeparam>
        /// <param name="key">Name of the key to read.</param>
        /// <returns>Value of the key which is converted to <typeparamref name="T" />.</returns>
        /// <remarks>Method doesn't catch any possible exceptions.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="key" /> isn't contained in the dictionary. -or-
        /// <paramref name="key" /> presents empty string.
        /// </exception>
        ///  <exception cref="InvalidCastException">
        /// This conversion is not supported. -or- Value is null and <typeparamref name="T" /> is a
        /// value type. -or- Value does not implement the <see cref="IConvertible" /> interface.
        /// </exception>
        /// <exception cref="FormatException">
        /// Value is not in a format recognized by <typeparamref name="T" />.
        /// </exception>
        /// <exception cref="OverflowException">
        /// Value represents a number that is out of the range of <typeparamref name="T" />.
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

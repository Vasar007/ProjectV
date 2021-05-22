using System;
using System.Collections.Generic;
using System.Linq;
using Acolyte.Assertions;
using Acolyte.Collections;

namespace ProjectV
{
    /// <summary>
    /// Provides access to read environment variable values. Service reads only specified 
    /// environment variable and parse it.
    /// </summary>
    public static class EnvironmentVariablesParser
    {
        /// <summary>
        /// Name of the environment variable to read.
        /// </summary>
        private static string EnvironmentVariableName { get; } = "ProjectV";

        /// <summary>
        /// Specifies the target where the variable is located.
        /// </summary>
        private static EnvironmentVariableTarget DefaultVariableTarget { get; } =
            EnvironmentVariableTarget.User;

        /// <summary>
        /// Stores parsed values from environment variable.
        /// </summary>
        private static readonly IReadOnlyDictionary<string, string> _values =
            ParseEnvironmentVariableValue(
                Environment.GetEnvironmentVariable(EnvironmentVariableName, DefaultVariableTarget)
            );


        /// <summary>
        /// Gets value from dictionary with parsed values.
        /// </summary>
        /// <param name="variableName">Variable name to get value.</param>
        /// <returns>Raw value of specified key.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="variableName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="variableName" /> isn't contained in the dictionary. -or-
        /// <paramref name="variableName" /> presents empty string.
        /// </exception>
        public static string GetValue(string variableName)
        {
            variableName.ThrowIfNullOrEmpty(nameof(variableName));

            if (!_values.TryGetValue(variableName, out string value))
            {
                throw new ArgumentException(
                    $"Variable name \"{variableName}\" was not found in the environment values.",
                    nameof(variableName)
                );
            }
            return value;
        }

        /// <summary>
        /// Gets value from dictionary and converts it to specified type.
        /// </summary>
        /// <typeparam name="T">Type to convert.</typeparam>
        /// <param name="variableName">Variable name to get value.</param>
        /// <returns>Converted value of specified key.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="variableName" /> isn't contained in the dictionary. -or-
        /// <paramref name="variableName" /> is <c>null</c> or presents empty.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// This conversion is not supported. -or- Value is <c>null</c> and conversion type is a
        /// value type. -or- Value does not implement the <see cref="IConvertible" /> interface.
        /// </exception>
        /// <exception cref="FormatException">
        /// Value is not in a format recognized by conversion type.
        /// </exception>
        /// <exception cref="OverflowException">
        /// Value represents a number that is out of the range of conversion type.
        /// </exception>
        /// <exception cref="ArgumentNullException">ConversionType is <c>null</c>.</exception>
        public static T GetValue<T>(string variableName)
            where T : IConvertible
        {
            string stringValue = GetValue(variableName);
            return (T)Convert.ChangeType(stringValue, typeof(T));
        }

        /// <summary>
        /// Tries to get value from dictionary and converts it to specified type or return default
        /// value.
        /// </summary>
        /// <typeparam name="T">Type to convert.</typeparam>
        /// <param name="variableName">Variable name to get value.</param>
        /// <returns>Converted value of specified key.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="variableName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="variableName" /> presents empty string.
        /// </exception>
        public static T GetValueOrDefault<T>(string variableName, T defaultValue)
            where T : IConvertible
        {
            variableName.ThrowIfNullOrEmpty(nameof(variableName));

            try
            {
                string stringValue = GetValue(variableName);
                return (T)Convert.ChangeType(stringValue, typeof(T));
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Parses environment variable value and converts it to dictionary of key-value pairs.
        /// </summary>
        /// <param name="environmentVariableValue">Raw value of environment variable.</param>
        /// <returns>Dictionary with parsed values.</returns>
        private static IReadOnlyDictionary<string, string> ParseEnvironmentVariableValue(
            string environmentVariableValue)
        {
            if (string.IsNullOrEmpty(environmentVariableValue))
            {
                return new Dictionary<string, string>();
            }

            IReadOnlyList<string> keyValuePairsRaw = environmentVariableValue.Split(';');

            return keyValuePairsRaw
                .Select(kv => ProcessKeyValuePair(kv))
                .ToReadOnlyDictionary(kv => kv.key, kv => kv.value);
        }

        /// <summary>
        /// Processes single key-value pair of string.
        /// </summary>
        /// <param name="rawKeyValuePair">String value of pair.</param>
        /// <returns>Parsed key and value packed in a tuple.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="paramName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="rawKeyValuePair" /> presents empty string.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="rawKeyValuePair" /> has invalid format. -or-
        /// <paramref name="rawKeyValuePair" /> has empty value among keys. -or-
        /// <paramref name="rawKeyValuePair" /> has empty value among values.
        /// </exception>
        private static (string key, string value) ProcessKeyValuePair(string rawKeyValuePair)
        {
            rawKeyValuePair.ThrowIfNullOrEmpty(nameof(rawKeyValuePair));

            IReadOnlyList<string> keyValuePair = rawKeyValuePair.Split('=');
            if (keyValuePair.Count != 2)
            {
                throw new InvalidOperationException(
                    $"Environment variable has invalid format: '{rawKeyValuePair}'."
                );
            }

            if (string.IsNullOrWhiteSpace(keyValuePair[0]))
            {
                throw new InvalidOperationException(
                    "Environment variable has empty value among keys: " +
                    $"'{keyValuePair[0]}'."
                );
            }
            if (string.IsNullOrWhiteSpace(keyValuePair[1]))
            {
                throw new InvalidOperationException(
                    "Environment variable has empty value among values: " +
                    $"'{keyValuePair[1]}'."
                );
            }

            return (key: keyValuePair[0], value: keyValuePair[1]);
        }
    }
}

using System;
using System.Collections.Generic;
using ThingAppraiser.Extensions;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Models.Internal
{
    public sealed class RawDataContainer
    {
        public IReadOnlyList<BasicInfo> RawData { get; }

        private readonly Dictionary<string, MinMaxDenominator> _additionalData =
            new Dictionary<string, MinMaxDenominator>();


        public RawDataContainer(IReadOnlyList<BasicInfo> rawData)
        {
            RawData = rawData.ThrowIfNull(nameof(rawData));
        }

        public bool AddParameter(string parameterName, MinMaxDenominator value)
        {
            parameterName.ThrowIfNullOrEmpty(nameof(parameterName));
            value.ThrowIfNull(nameof(value));

            if (_additionalData.ContainsKey(parameterName)) return false;

            _additionalData.Add(parameterName, value);
            return true;
        }

        public MinMaxDenominator GetParameter(string parameterName)
        {
            parameterName.ThrowIfNullOrEmpty(nameof(parameterName));

            if (!_additionalData.TryGetValue(parameterName, out MinMaxDenominator result))
            {
                throw new ArgumentException(
                    "Parameter with specified name does not exist in parameter collection.",
                    nameof(parameterName)
                );
            }
            return result;
        }
    }
}

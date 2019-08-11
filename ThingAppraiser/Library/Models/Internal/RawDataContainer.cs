using System;
using System.Collections.Generic;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Models.Internal
{
    public class RawDataContainer
    {
        private readonly List<BasicInfo> _rawData;

        private readonly Dictionary<string, MinMaxDenominator> _additionalData =
            new Dictionary<string, MinMaxDenominator>();


        public RawDataContainer(List<BasicInfo> rawData)
        {
            _rawData = rawData.ThrowIfNull(nameof(rawData));
        }

        public IReadOnlyList<BasicInfo> GetData()
        {
            return _rawData;
        }

        public bool AddParameter(string parameterName, MinMaxDenominator value)
        {
            if (_additionalData.ContainsKey(parameterName)) return false;

            _additionalData.Add(parameterName, value);
            return true;
        }

        public MinMaxDenominator GetParameter(string parameterName)
        {
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

using System;
using System.Collections.Generic;

namespace ThingAppraiser.Data
{
    public class CRawDataContainer
    {
        private readonly List<CBasicInfo> _rawData;

        private readonly Dictionary<String, CMinMaxDenominator> _additionalData =
            new Dictionary<String, CMinMaxDenominator>();


        public CRawDataContainer(List<CBasicInfo> rawData)
        {
            _rawData = rawData.ThrowIfNull(nameof(rawData));
        }

        public IReadOnlyList<CBasicInfo> GetData()
        {
            return _rawData;
        }

        public Boolean AddParameter(String parameterName, CMinMaxDenominator value)
        {
            if (_additionalData.ContainsKey(parameterName)) return false;

            _additionalData.Add(parameterName, value);
            return true;
        }

        public CMinMaxDenominator GetParameter(String parameterName)
        {
            if (!_additionalData.TryGetValue(parameterName, out CMinMaxDenominator result))
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

using System.Collections.Generic;
using Acolyte.Assertions;

namespace ProjectV.IO.Input.WebService
{
    public sealed class InputTransmitter : IInputter, ITagable
    {
        private readonly IEnumerable<string> _thingNames;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(InputTransmitter);

        #endregion

        public string StorageName { get; private set; } = string.Empty;


        public InputTransmitter(IEnumerable<string> thingNames)
        {
            _thingNames = thingNames.ThrowIfNull(nameof(thingNames));
        }

        #region IInputter Implementation

        public IEnumerable<string> ReadThingNames(string storageName)
        {
            StorageName = storageName;

            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var result = new HashSet<string>();
            foreach (string thingName in _thingNames)
            {
                if (result.Add(thingName))
                {
                    yield return thingName;
                }
            }
        }

        #endregion
    }
}

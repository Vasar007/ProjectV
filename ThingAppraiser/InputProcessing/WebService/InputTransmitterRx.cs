using System.Collections.Generic;

namespace ThingAppraiser.IO.Input.WebService
{
    public class InputTransmitterRx : IInputterRx, IInputterBase, ITagable
    {
        private readonly List<string> _thingNames;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = "InputTransmitterRx";

        #endregion

        public string StorageName { get; private set; }


        public InputTransmitterRx(List<string> thingNames)
        {
            _thingNames = thingNames.ThrowIfNull(nameof(thingNames));
        }

        #region IInputterRx Implementation

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

using System.Collections.Generic;
using System.Linq;

namespace ThingAppraiser.IO.Input.WebService
{
    public sealed class InputTransmitter : IInputter, IInputterBase, ITagable
    {
        private readonly List<string> _thingNames;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(InputTransmitter);

        #endregion

        public string StorageName { get; private set; }


        public InputTransmitter(List<string> thingNames)
        {
            _thingNames = thingNames.ThrowIfNull(nameof(thingNames));
        }

        #region IInputter Implementation

        public List<string> ReadThingNames(string storageName)
        {
            StorageName = storageName;

            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var result = new HashSet<string>();
            foreach (string thingName in _thingNames)
            {
                result.Add(thingName);
            }
            return result.ToList();
        }

        #endregion
    }
}

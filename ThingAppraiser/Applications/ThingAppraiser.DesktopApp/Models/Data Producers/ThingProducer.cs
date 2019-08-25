using System.Collections.Generic;
using ThingAppraiser.IO.Input;

namespace ThingAppraiser.DesktopApp.Models.DataProducers
{
    internal sealed class ThingProducer : IInputter, ITagable
    {
        private readonly List<string> _thingNames;

        public string StorageName { get; private set; } = string.Empty;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = "ThingProducer";

        #endregion


        public ThingProducer(List<string> thingNames)
        {
            _thingNames = thingNames.ThrowIfNull(nameof(thingNames));
        }

        #region IInputter Implementation

        public List<string> ReadThingNames(string storageName)
        {
            StorageName = storageName.ThrowIfNull(nameof(storageName));
            return _thingNames;
        }

        #endregion
    }
}

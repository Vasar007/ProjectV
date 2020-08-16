using System.Collections.Generic;
using Acolyte.Assertions;
using ProjectV.IO.Input;

namespace ProjectV.DesktopApp.Models.DataProducers
{
    internal sealed class ThingProducer : IInputterAsync, ITagable
    {
        private readonly IEnumerable<string> _thingNames;

        public string StorageName { get; private set; }

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(ThingProducer);

        #endregion


        public ThingProducer(IEnumerable<string> thingNames)
        {
            _thingNames = thingNames.ThrowIfNull(nameof(thingNames));

            StorageName = string.Empty;
        }

        #region IInputter Implementation

        public IEnumerable<string> ReadThingNames(string storageName)
        {
            StorageName = storageName.ThrowIfNull(nameof(storageName));
            return _thingNames;
        }

        #endregion
    }
}

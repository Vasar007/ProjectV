using System.Collections.Generic;
using Acolyte.Assertions;
using ProjectV.IO.Input;

namespace ProjectV.DesktopApp.Models.DataProducers
{
    internal sealed class ThingProducer : IInputter, ITagable
    {
        private readonly IReadOnlyList<string> _thingNames;

        public string StorageName { get; private set; } = string.Empty;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = "ThingProducer";

        #endregion


        public ThingProducer(IReadOnlyList<string> thingNames)
        {
            _thingNames = thingNames.ThrowIfNull(nameof(thingNames));
        }

        #region IInputter Implementation

        public IReadOnlyList<string> ReadThingNames(string storageName)
        {
            StorageName = storageName.ThrowIfNull(nameof(storageName));
            return _thingNames;
        }

        #endregion
    }
}

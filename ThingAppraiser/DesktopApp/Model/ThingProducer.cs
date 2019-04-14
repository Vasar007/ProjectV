using System;
using System.Collections.Generic;
using ThingAppraiser;
using ThingAppraiser.IO.Input;

namespace DesktopApp.Model.DataProducers
{
    public class CThingProducer : IInputter, ITagable
    {
        private readonly List<String> _thingsNamesList;

        public String StorageName { get; private set; }

        #region ITagable Implementation

        /// <inheritdoc />
        public String Tag { get; } = "ThingProducer";

        #endregion


        public CThingProducer(List<String> thingsNamesList)
        {
            _thingsNamesList = thingsNamesList.ThrowIfNull(nameof(thingsNamesList));
        }

        #region IInputter Implementation

        public List<String> ReadThingNames(String storageName)
        {
            StorageName = storageName;
            return _thingsNamesList;
        }

        #endregion
    }
}

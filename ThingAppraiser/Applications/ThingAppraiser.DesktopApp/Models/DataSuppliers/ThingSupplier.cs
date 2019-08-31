using System;
using System.Linq;
using System.Collections.Generic;
using ThingAppraiser.Models.WebService;
using ThingAppraiser.DesktopApp.Models.Things;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.DesktopApp.Models.DataSuppliers
{
    internal sealed class ThingSupplier : IThingSupplier, ITagable
    {
        private readonly IThingGrader _thingGrader;

        private readonly List<Thing> _things = new List<Thing>();

        public string StorageName { get; private set; } = string.Empty;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = "ThingSupplier";

        #endregion


        public ThingSupplier(IThingGrader thingGrader)
        {
            _thingGrader = thingGrader.ThrowIfNull(nameof(thingGrader));
        }

        #region IThingSupplier Implementation

        public IReadOnlyList<Thing> GetAllThings()
        {
            return _things;
        }

        public Thing GetThingById(Guid thingId)
        {
            return _things.First(p => p.InternalId.Equals(thingId));
        }

        public bool SaveResults(ProcessingResponse response, string storageName)
        {
            StorageName = storageName;

            _thingGrader.ProcessMetadata(response.Metadata);

            if (_things.Count > 0)
            {
                _things.Clear();
            }
            foreach (IReadOnlyList<RatingDataContainer> rating in response.RatingDataContainers)
            {
                _things.AddRange(_thingGrader.ProcessRatings(rating));
            }
            return true;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Data;

namespace ThingAppraiser.DesktopApp.Models.DataSuppliers
{
    internal class ThingSupplier : IThingSupplier, ITagable
    {
        private readonly List<Thing> _things = new List<Thing>();

        public string StorageName { get; private set; }

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = "ThingSupplier";

        #endregion


        public ThingSupplier()
        {
        }

        #region IThingSupplier Implementation

        public List<Thing> GetAllThings()
        {
            return _things;
        }

        public Thing GetThingById(Guid thingId)
        {
            return _things.Find(p => p.InternalId.Equals(thingId));
        }

        #endregion

        public bool SaveResults(List<List<RatingDataContainer>> results, string storageName,
            IImageSupplier imageSupplier)
        {
            imageSupplier.ThrowIfNull(nameof(imageSupplier));

            StorageName = storageName;

            if (_things.Count != 0)
            {
                _things.Clear();
            }
            foreach (List<RatingDataContainer> rating in results)
            {
                _things.AddRange(rating.Select(r => 
                    new Thing(
                        Guid.NewGuid(), r.DataHandler,
                        imageSupplier.GetImageLink(r.DataHandler, ImageSize.Large))
                    )
                );
            }
            return true;
        }
    }
}

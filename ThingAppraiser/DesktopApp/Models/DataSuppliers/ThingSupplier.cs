using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Data;
using ThingAppraiser.IO.Output;

namespace ThingAppraiser.DesktopApp.Models.DataSuppliers
{
    public class ThingSupplier : IThingSupplier, IOutputter, IOutputterBase, ITagable
    {
        private readonly List<Thing> _things = new List<Thing>();

        private readonly IImageSupplier _imageSupplier;

        public string StorageName { get; private set; }

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = "ThingSupplier";

        #endregion


        public ThingSupplier(IImageSupplier imageSupplier)
        {
            _imageSupplier = imageSupplier.ThrowIfNull(nameof(imageSupplier));
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

        #region IOutputter Implementation

        public bool SaveResults(List<List<RatingDataContainer>> results, string storageName)
        {
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
                        _imageSupplier.GetImageLink(r.DataHandler, ImageSize.Large))
                    )
                );
            }
            return true;
        }

        #endregion
    }
}

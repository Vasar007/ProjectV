using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser;
using ThingAppraiser.Data;
using ThingAppraiser.IO.Output;

namespace DesktopApp.Model.DataSuppliers
{
    public class CThingSupplier : IThingSupplier, IOutputter, ITagable
    {
        private readonly List<CThing> _things = new List<CThing>();

        private readonly IImageSupplier _imageSupplier;

        public String StorageName { get; private set; }

        #region ITagable Implementation

        /// <inheritdoc />
        public String Tag { get; } = "ThingSupplier";

        #endregion


        public CThingSupplier(IImageSupplier imageSupplier)
        {
            _imageSupplier = imageSupplier.ThrowIfNull(nameof(imageSupplier));
        }

        #region IThingSupplier Implementation

        public List<CThing> GetAllThings()
        {
            return _things;
        }

        public CThing GetThingByID(Guid thingID)
        {
            return _things.Find(p => p.InternalID.Equals(thingID));
        }

        #endregion

        #region IOutputter Implementation

        public Boolean SaveResults(List<List<CRatingDataContainer>> results, String storageName)
        {
            StorageName = storageName;

            if (_things.Count != 0)
            {
                _things.Clear();
            }
            foreach (List<CRatingDataContainer> rating in results)
            {
                _things.AddRange(rating.Select(r => 
                    new CThing(
                        Guid.NewGuid(), r.DataHandler, 
                        _imageSupplier.GetImageLink(r.DataHandler, EImageSize.Large))
                    )
                );
            }
            return true;
        }

        #endregion
    }
}

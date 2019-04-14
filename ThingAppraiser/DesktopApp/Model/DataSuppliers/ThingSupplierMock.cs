using System;
using System.Collections.Generic;
using ThingAppraiser.Data;

namespace DesktopApp.Model.DataSuppliers
{
    public class CThingSupplierMock : IThingSupplier
    {
        private readonly List<CThing> _things;


        public CThingSupplierMock()
        {
            _things = new List<CThing>
            {
                // CBasicInfo => Title, ID, VoteCount, VoteAverage.
                new CThing(Guid.NewGuid(), new CBasicInfo("Movie One",    1, 100, 1.0f),
                           "https://dummyimage.com/300.png"),
                new CThing(Guid.NewGuid(), new CBasicInfo("Movie Two",    2, 200, 2.0f), 
                           "https://dummyimage.com/300.png"),
                new CThing(Guid.NewGuid(), new CBasicInfo("Movie Three",  3, 300, 3.0f), 
                           "https://dummyimage.com/300.png"),
                new CThing(Guid.NewGuid(), new CBasicInfo("Movie Four",   4, 400, 4.0f), 
                           "https://dummyimage.com/300.png"),
                new CThing(Guid.NewGuid(), new CBasicInfo("Movie Five",   5, 500, 5.0f), 
                           "https://dummyimage.com/300.png"),
                new CThing(Guid.NewGuid(), new CBasicInfo("Movie Six",    6, 600, 6.0f), 
                           "https://dummyimage.com/300.png"),
                new CThing(Guid.NewGuid(), new CBasicInfo("Movie Seven",  7, 700, 7.0f), 
                           "https://dummyimage.com/300.png"),
                new CThing(Guid.NewGuid(), new CBasicInfo("Movie Eight",  8, 800, 8.0f), 
                           "https://dummyimage.com/300.png"),
                new CThing(Guid.NewGuid(), new CBasicInfo("Movie Nine",   9, 900, 9.0f), 
                           "https://dummyimage.com/300.png"),
                new CThing(Guid.NewGuid(), new CBasicInfo("Movie Ten",   10, 999, 9.9f), 
                           "https://dummyimage.com/300.png")
            };
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
    }
}

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
                new CThing(Guid.NewGuid(), new CBasicInfo(1,  "Movie One",   100, 1.0),
                           "https://dummyimage.com/300.png"),
                new CThing(Guid.NewGuid(), new CBasicInfo(2,  "Movie Two",   200, 2.0), 
                           "https://dummyimage.com/300.png"),
                new CThing(Guid.NewGuid(), new CBasicInfo(3,  "Movie Three", 300, 3.0), 
                           "https://dummyimage.com/300.png"),
                new CThing(Guid.NewGuid(), new CBasicInfo(4,  "Movie Four",  400, 4.0), 
                           "https://dummyimage.com/300.png"),
                new CThing(Guid.NewGuid(), new CBasicInfo(5,  "Movie Five",  500, 5.0), 
                           "https://dummyimage.com/300.png"),
                new CThing(Guid.NewGuid(), new CBasicInfo(6,  "Movie Six",   600, 6.0), 
                           "https://dummyimage.com/300.png"),
                new CThing(Guid.NewGuid(), new CBasicInfo(7,  "Movie Seven", 700, 7.0), 
                           "https://dummyimage.com/300.png"),
                new CThing(Guid.NewGuid(), new CBasicInfo(8,  "Movie Eight", 800, 8.0), 
                           "https://dummyimage.com/300.png"),
                new CThing(Guid.NewGuid(), new CBasicInfo(9,  "Movie Nine",  900, 9.0), 
                           "https://dummyimage.com/300.png"),
                new CThing(Guid.NewGuid(), new CBasicInfo(10, "Movie Ten",   999, 9.9), 
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

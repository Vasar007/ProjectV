using System;
using System.Linq;
using System.Collections.Generic;
using ProjectV.DesktopApp.Models.Things;
using ProjectV.Models.Data;
using ProjectV.Models.WebService;

namespace ProjectV.DesktopApp.Models.DataSuppliers
{
    internal sealed class ThingSupplierMock : IThingSupplier, ITagable
    {
        private readonly List<Thing> _things;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = "ThingSupplierMock";

        #endregion


        public ThingSupplierMock()
        {
            _things = new List<Thing>
            {
                // BasicInfo => Title, ID, VoteCount, VoteAverage.
                new Thing(Guid.NewGuid(), new BasicInfo(1,  "Movie One",   100, 1.0),
                          "https://dummyimage.com/300.png"),

                new Thing(Guid.NewGuid(), new BasicInfo(2,  "Movie Two",   200, 2.0), 
                          "https://dummyimage.com/300.png"),

                new Thing(Guid.NewGuid(), new BasicInfo(3,  "Movie Three", 300, 3.0), 
                          "https://dummyimage.com/300.png"),

                new Thing(Guid.NewGuid(), new BasicInfo(4,  "Movie Four",  400, 4.0), 
                          "https://dummyimage.com/300.png"),

                new Thing(Guid.NewGuid(), new BasicInfo(5,  "Movie Five",  500, 5.0), 
                          "https://dummyimage.com/300.png"),

                new Thing(Guid.NewGuid(), new BasicInfo(6,  "Movie Six",   600, 6.0), 
                          "https://dummyimage.com/300.png"),

                new Thing(Guid.NewGuid(), new BasicInfo(7,  "Movie Seven", 700, 7.0), 
                          "https://dummyimage.com/300.png"),

                new Thing(Guid.NewGuid(), new BasicInfo(8,  "Movie Eight", 800, 8.0), 
                          "https://dummyimage.com/300.png"),

                new Thing(Guid.NewGuid(), new BasicInfo(9,  "Movie Nine",  900, 9.0), 
                          "https://dummyimage.com/300.png"),

                new Thing(Guid.NewGuid(), new BasicInfo(10, "Movie Ten",   999, 9.9), 
                          "https://dummyimage.com/300.png")
            };
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
            return false;
        }

        #endregion
    }
}

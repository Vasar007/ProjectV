using System;
using System.Collections.Generic;
using ThingAppraiser.Data.Models;
using ThingAppraiser.DesktopApp.Models.Things;

namespace ThingAppraiser.DesktopApp.Models.DataSuppliers
{
    internal interface IThingSupplier : ITagable
    {
        List<Thing> GetAllThings();

        Thing GetThingById(Guid thingId);

        bool SaveResults(ProcessingResponse response, string storageName);
    }
}

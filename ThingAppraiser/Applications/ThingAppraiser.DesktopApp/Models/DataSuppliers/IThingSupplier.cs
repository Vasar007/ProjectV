using System;
using System.Collections.Generic;
using ThingAppraiser.Models.WebService;
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

using System;
using System.Collections.Generic;

namespace ThingAppraiser.DesktopApp.Models.DataSuppliers
{
    public interface IThingSupplier
    {
        List<Thing> GetAllThings();

        Thing GetThingById(Guid thingId);
    }
}

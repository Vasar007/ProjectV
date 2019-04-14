using System;
using System.Collections.Generic;

namespace DesktopApp.Model.DataSuppliers
{
    public interface IThingSupplier
    {
        List<CThing> GetAllThings();

        CThing GetThingByID(Guid thingID);
    }
}

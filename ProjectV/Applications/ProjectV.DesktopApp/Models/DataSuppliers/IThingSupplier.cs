using System;
using System.Collections.Generic;
using ProjectV.Models.WebService;
using ProjectV.DesktopApp.Models.Things;

namespace ProjectV.DesktopApp.Models.DataSuppliers
{
    internal interface IThingSupplier : ITagable
    {
        IReadOnlyList<Thing> GetAllThings();

        Thing GetThingById(Guid thingId);

        bool SaveResults(ProcessingResponse response, string storageName);
    }
}

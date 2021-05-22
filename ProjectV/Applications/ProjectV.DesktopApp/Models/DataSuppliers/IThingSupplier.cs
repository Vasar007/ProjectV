using System;
using System.Collections.Generic;
using ProjectV.DesktopApp.Models.Things;
using ProjectV.Models.WebService;

namespace ProjectV.DesktopApp.Models.DataSuppliers
{
    internal interface IThingSupplier : ITagable
    {
        IReadOnlyList<Thing> GetAllThings();

        Thing GetThingById(Guid thingId);

        bool SaveResults(ProcessingResponse response, string storageName);
    }
}

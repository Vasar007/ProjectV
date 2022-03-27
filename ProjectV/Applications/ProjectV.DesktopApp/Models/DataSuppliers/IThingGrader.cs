using System.Collections.Generic;
using ProjectV.DesktopApp.Models.Things;
using ProjectV.Models.Internal;
using ProjectV.Models.WebService.Responses;

namespace ProjectV.DesktopApp.Models.DataSuppliers
{
    internal interface IThingGrader
    {
        IReadOnlyList<Thing> ProcessRatings(IReadOnlyList<RatingDataContainer> rating);

        void ProcessMetadata(ProcessingResponseMetadata metadata);
    }
}

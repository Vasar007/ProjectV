using System.Collections.Generic;
using ThingAppraiser.Models.WebService;
using ThingAppraiser.DesktopApp.Models.Things;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.DesktopApp.Models.DataSuppliers
{
    internal interface IThingGrader
    {
        IReadOnlyList<Thing> ProcessRatings(IReadOnlyList<RatingDataContainer> rating);

        void ProcessMetadata(ResponseMetadata metadata);
    }
}

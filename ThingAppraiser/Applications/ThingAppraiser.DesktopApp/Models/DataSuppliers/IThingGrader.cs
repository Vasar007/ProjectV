using System.Collections.Generic;
using ThingAppraiser.Models.WebService;
using ThingAppraiser.DesktopApp.Models.Things;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.DesktopApp.Models.DataSuppliers
{
    internal interface IThingGrader
    {
        List<Thing> ProcessRatings(List<RatingDataContainer> rating);

        void ProcessMetadata(ResponseMetadata metadata);
    }
}

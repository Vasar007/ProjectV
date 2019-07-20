using System.Collections.Generic;
using ThingAppraiser.Data;
using ThingAppraiser.Data.Models;
using ThingAppraiser.DesktopApp.Models.Things;

namespace ThingAppraiser.DesktopApp.Models.DataSuppliers
{
    internal interface IThingGrader
    {
        List<Thing> ProcessRatings(List<RatingDataContainer> rating);

        void ProcessMetaData(ResponseMetaData metaData);
    }
}

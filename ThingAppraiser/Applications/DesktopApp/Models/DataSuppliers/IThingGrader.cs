using System.Collections.Generic;
using ThingAppraiser.Data;
using ThingAppraiser.Data.Models;

namespace ThingAppraiser.DesktopApp.Models.DataSuppliers
{
    internal interface IThingGrader
    {
        List<Thing> ProcessRatings(List<RatingDataContainer> rating);

        void ProcessMetaData(ResponseMetaData metaData);
    }
}

using System;
using System.Collections.Generic;

namespace ThingAppraiser.Appraisers
{
    public abstract class MoviesAppraiser : Appraiser
    {
        public override Type TypeID { get { return typeof(Data.Movie); } }

        public override List<(Data.DataHandler, float)> GetRatings(List<Data.DataHandler> entities)
        {
            return base.GetRatings(entities);
        }
    }
}

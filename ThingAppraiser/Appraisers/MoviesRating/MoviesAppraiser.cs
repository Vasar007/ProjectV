using System;
using System.Collections.Generic;

namespace ThingAppraiser.Appraisers
{
    public abstract class MoviesAppraiser : Appraiser
    {
        public override Type type { get { return typeof(Data.Movie); } }

        public override List<Tuple<Data.DataHandler, float>>
            GetRatings(List<Data.DataHandler> entities)
        {
            return base.GetRatings(entities);
        }
    }
}

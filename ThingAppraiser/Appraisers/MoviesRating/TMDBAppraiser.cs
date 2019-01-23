using System;
using System.Collections.Generic;

namespace ThingAppraiser.Appraisers
{
    public class TMDBAppraiser : MoviesAppraiser
    {
        public override Type type { get { return typeof(Data.TMDBMovie); } }

        public override List<Tuple<Data.DataHandler, float>>
            GetRatings(List<Data.DataHandler> entities)
        {
            return base.GetRatings(entities);
        }
    }
}

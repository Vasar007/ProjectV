using System;
using System.Collections.Generic;
using System.Linq;

namespace ThingAppraiser.Appraisers
{
    public abstract class Appraiser<T> where T : Data.DataHandler
    {
        public virtual List<T> GetRatings(List<T> entities)
        {
            return entities.OrderBy(entity => entity.Vote_Average)
                .ThenBy(entity => entity.Vote_Count).ToList();
        }
    }
}

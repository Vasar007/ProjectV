using System;
using System.Collections.Generic;
using System.Linq;

namespace ThingAppraiser.Appraisers
{
    public abstract class Appraiser
    {
        public virtual Type type { get { return typeof(Data.DataHandler); } }

        public virtual List<Tuple<Data.DataHandler, float>>
            GetRatings(List<Data.DataHandler> entities)
        {
            var ratings = new List<Tuple<Data.DataHandler, float>>();

            var maxValueVA = entities.Max(e => e.Vote_Average);
            var minValueVA = entities.Min(e => e.Vote_Average);
            var denominatorVA = maxValueVA - minValueVA;

            var maxValueVC = entities.Max(e => e.Vote_Count);
            var minValueVC = entities.Min(e => e.Vote_Count);
            var denominatorVC = (float)maxValueVC - minValueVC;

            foreach (var entity in entities)
            {
                var normValueVA = (entity.Vote_Average - minValueVA) / denominatorVA;
                var normValueVC = (entity.Vote_Count - minValueVC) / denominatorVC;
                ratings.Add(new Tuple<Data.DataHandler, float>(entity, normValueVA + normValueVC));
                Console.WriteLine($"{normValueVA} : {entity.Vote_Average}; \t {normValueVC} : {entity.Vote_Count}");
            }

            ratings.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            return ratings;
        }
    }
}

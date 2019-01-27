using System;
using System.Collections.Generic;
using MoreLinq;

namespace ThingAppraiser.Appraisers
{
    public abstract class Appraiser
    {
        public virtual Type TypeID { get { return typeof(Data.DataHandler); } }

        public virtual List<(Data.DataHandler, float)> GetRatings(List<Data.DataHandler> entities)
        {
            var ratings = new List<(Data.DataHandler, float)>();

            var normalizerVA = new Normalizer<float, Data.DataHandler>(entities,
                                                                       e => e.Vote_Average);
            var normalizerVC = new Normalizer<uint, Data.DataHandler>(entities, e => e.Vote_Count);

            var enumerator = entities.ZipShortest(normalizerVA.Normalize(),
                                                   normalizerVC.Normalize(),
                                                   (t1, t2, t3) => (t1, t2, t3));
            foreach (var (entity, normValueVA, normValueVC) in enumerator)
            {
                ratings.Add((entity, normValueVA + normValueVC));
                //Console.WriteLine($"{normValueVA} : {entity.Vote_Average}; \t " +
                //                  $"{normValueVC} : {entity.Vote_Count}");
            }

            ratings.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            return ratings;
        }
    }
}

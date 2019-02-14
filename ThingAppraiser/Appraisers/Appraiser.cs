using System;
using System.Collections.Generic;
using MoreLinq;

namespace ThingAppraiser.Appraisers
{
    public abstract class Appraiser
    {
        public virtual Type TypeID { get { return typeof(Data.DataHandler); } }

        public virtual List<Data.ResultType> GetRatings(List<Data.DataHandler> entities)
        {
            var ratings = new List<Data.ResultType>();

            var normalizerVA = new Normalizer<float, Data.DataHandler>(entities,
                                                                       e => e.Vote_Average);
            var normalizerVC = new Normalizer<uint, Data.DataHandler>(entities, e => e.Vote_Count);

            var enumerator = entities.ZipShortest(normalizerVA.Normalize(),
                                                   normalizerVC.Normalize(),
                                                   (t1, t2, t3) => (t1, t2, t3));
            foreach (var (entity, normValueVA, normValueVC) in enumerator)
            {
                ratings.Add(new Data.ResultType(entity, normValueVA + normValueVC));
                //Shell.Shell.OutputMessage($"{normValueVA} : {entity.Vote_Average}; \t " +
                //                          $"{normValueVC} : {entity.Vote_Count}");
            }

            ratings.Sort((x, y) => y.RatingValue.CompareTo(x.RatingValue));
            return ratings;
        }
    }
}

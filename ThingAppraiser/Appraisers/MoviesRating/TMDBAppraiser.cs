using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace ThingAppraiser.Appraisers
{
    public class TMDBAppraiser : MoviesAppraiser
    {
        public override Type TypeID { get { return typeof(Data.TMDBMovie); } }

        public override List<(Data.DataHandler, float)> GetRatings(List<Data.DataHandler> entities)
        {
            if (entities == null || entities.Count == 0) return null;

            // Check if list have proper type.
            if (!entities.All(e => e is Data.TMDBMovie))
            {
                throw new ArgumentException("Element type is not valid for this appraiser.");
            }

            var ratings = new List<(Data.DataHandler, float)>();
            var converted = entities.ConvertAll(e => (Data.TMDBMovie)e);

            var normalizerVA = new Normalizer<float, Data.TMDBMovie>(converted,
                                                                     c => c.Vote_Average);
            var normalizerVC = new Normalizer<uint, Data.TMDBMovie>(converted, c => c.Vote_Count);
            var normalizerPopularity = new Normalizer<float, Data.TMDBMovie>(converted,
                                                                             c => c.Popularity);

            // Use additional property.
            var enumerator = converted.ZipShortest(normalizerVA.Normalize(),
                                                   normalizerVC.Normalize(),
                                                   normalizerPopularity.Normalize(),
                                                   (t1, t2, t3, t4) => (t1, t2, t3, t4));
            foreach (var (entity, normValueVA, normValueVC, normValuePopularity) in enumerator)
            {
                ratings.Add((entity, normValueVA + normValueVC + normValuePopularity));
                //Console.WriteLine($"{normValueVA} : {entity.Vote_Average}; \t " +
                //                  $"{normValueVC} : {entity.Vote_Count}; \t " +
                //                  $"{normValuePopularity} : {entity.Popularity}");
            }

            ratings.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            return ratings;
        }
    }
}

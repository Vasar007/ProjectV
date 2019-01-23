using System;
using System.Collections.Generic;

namespace ThingAppraiser.Appraisers
{
    public class AppraisersManager
    {
        public Dictionary<Type, List<Appraiser>> appraisers = 
            new Dictionary<Type, List<Appraiser>>();

        public void Add(Appraiser appraiser)
        {
            if (appraisers.ContainsKey(appraiser.type))
            {
                var list = appraisers[appraiser.type];
                if (!list.Contains(appraiser))
                {
                    list.Add(appraiser);
                }
            }
            else
            {
                var list = new List<Appraiser>
                {
                    appraiser
                };
                appraisers.Add(appraiser.type, list);
            }
        }

        public List<List<Tuple<Data.DataHandler, float>>>
            GetAllRatings(List<List<Data.DataHandler>> data)
        {
            var results = new List<List<Tuple<Data.DataHandler, float>>>();
            foreach (var datum in data)
            {
                if (datum.Count == 0) continue;
                if (!appraisers.TryGetValue(datum[0].GetType(), out var values))
                {
                    Console.WriteLine($"Type {datum[0].GetType()} was not used!");
                    continue;
                }
                foreach (var appraiser in values)
                {
                    results.Add(appraiser.GetRatings(datum));
                }
            }
            return results;
        }
    }
}

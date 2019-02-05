using System;
using System.Collections;
using System.Collections.Generic;

namespace ThingAppraiser.Appraisers
{
    public class AppraisersManager : IEnumerable
    {
        private Dictionary<Type, List<Appraiser>> _appraisers =
            new Dictionary<Type, List<Appraiser>>();

        public void Add(Appraiser appraiser)
        {
            if (_appraisers.ContainsKey(appraiser.TypeID))
            {
                var list = _appraisers[appraiser.TypeID];
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
                _appraisers.Add(appraiser.TypeID, list);
            }
        }

        public List<List<Data.ResultType>>
            GetAllRatings(List<List<Data.DataHandler>> data)
        {
            var results = new List<List<Data.ResultType>>();
            foreach (var datum in data)
            {
                if (datum.Count == 0) continue;
                if (!_appraisers.TryGetValue(datum[0].GetType(), out var values))
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

        public static void PrintRatingsToConsole(List<List<Data.ResultType>> ratings)
        {
            foreach (var rating in ratings)
            {
                foreach (var (item, value) in rating)
                {
                    Console.WriteLine($"{item.Title} has rating {value}.");
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _appraisers.GetEnumerator();
        }
    }
}

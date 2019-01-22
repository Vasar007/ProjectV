using System;
using System.Collections.Generic;

namespace FilmsEvaluator.Crawlers
{
    [Serializable()]
    public class TMDBMovie : Movie
    {
        public float Popularity { get; set; }
        public bool Adult { get; set; }
        public List<int> Genre_IDs { get; set; }
    }
}

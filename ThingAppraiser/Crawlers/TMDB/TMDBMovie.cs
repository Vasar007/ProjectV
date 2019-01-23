using System;
using System.Collections.Generic;

namespace ThingAppraiser.Data
{
    [Serializable()]
    public class TMDBMovie : Data.Movie
    {
        public float Popularity { get; set; }
        public bool Adult { get; set; }
        public List<int> Genre_IDs { get; set; }
    }
}

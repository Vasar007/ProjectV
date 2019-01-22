using System;

namespace FilmsEvaluator.Crawlers
{
    [Serializable()]
    public class Movie
    {
        public string Title { get; set; }
        public int ID { get; set; }
        public uint Vote_Count { get; set; }
        public float Vote_Average { get; set; }
        public string Overview { get; set; }
        public DateTime Release_Date { get; set; }
    }
}

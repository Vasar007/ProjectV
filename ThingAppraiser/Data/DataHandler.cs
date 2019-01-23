using System;

namespace ThingAppraiser.Data
{
    [Serializable()]
    public class DataHandler
    {
        public string Title { get; set; }
        public int ID { get; set; }
        public uint Vote_Count { get; set; }
        public float Vote_Average { get; set; }
    }
}

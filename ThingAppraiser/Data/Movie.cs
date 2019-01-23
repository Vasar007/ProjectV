using System;

namespace ThingAppraiser.Data
{
    [Serializable()]
    public class Movie : DataHandler
    {
        public string Overview { get; set; }
        public DateTime Release_Date { get; set; }
    }
}

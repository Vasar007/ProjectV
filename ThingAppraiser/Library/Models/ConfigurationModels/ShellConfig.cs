using System;
using System.Xml.Serialization;

namespace ThingAppraiser.Data.Configuration
{
    [Serializable]
    public class ShellConfig
    {
        [XmlElement]
        public MessageHandlerConfig MessageHandler { get; set; }

        [XmlElement]
        public InputManagerConfig InputManager { get; set; }

        [XmlElement]
        public CrawlersManagerConfig CrawlersManager { get; set; }

        [XmlElement]
        public AppraisersManagerConfig AppraisersManager { get; set; }

        [XmlElement]
        public OutputManagerConfig OutputManager { get; set; }

        [XmlElement]
        public DataBaseManagerConfig DataBaseManager { get; set; }


        public ShellConfig()
        {
        }
    }
}

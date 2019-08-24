using System;
using System.Xml.Serialization;

namespace ThingAppraiser.Models.Configuration
{
    // TODO: make this DTO immutable.
    [Serializable]
    public sealed class ShellConfig
    {
        [XmlElement]
        public MessageHandlerConfig MessageHandler { get; set; } = default!;

        [XmlElement]
        public InputManagerConfig InputManager { get; set; } = default!;

        [XmlElement]
        public CrawlersManagerConfig CrawlersManager { get; set; } = default!;

        [XmlElement]
        public AppraisersManagerConfig AppraisersManager { get; set; } = default!;

        [XmlElement]
        public OutputManagerConfig OutputManager { get; set; } = default!;

        [XmlElement]
        public DataBaseManagerConfig DataBaseManager { get; set; } = default!;


        public ShellConfig()
        {
        }
    }
}

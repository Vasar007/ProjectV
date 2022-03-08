﻿using System.Xml.Serialization;

namespace ProjectV.Models.Configuration
{
    // TODO: make this DTO immutable.
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
        public DatabaseManagerConfig DataBaseManager { get; set; } = default!;


        public ShellConfig()
        {
        }
    }
}

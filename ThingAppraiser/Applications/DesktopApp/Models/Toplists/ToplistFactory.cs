﻿using System;
using System.IO;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    internal static class ToplistFactory
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(ToplistFactory));

        public static ToplistBase Create(string toplistName, ToplistType toplistType,
            ToplistFormat toplistFormat)
        {
            toplistName.ThrowIfNullOrEmpty(nameof(toplistName));

            switch (toplistType)
            {
                case ToplistType.Score:
                    return new ScoreToplist(toplistName, toplistFormat);

                case ToplistType.Simple:
                    return new SimpleToplist(toplistName, toplistFormat);

                default:
                    throw new ArgumentOutOfRangeException(nameof(toplistType), toplistType,
                                                          "Could not recognize toplist type.");
            }
        }

        public static ToplistBase LoadFromFile(string toplistFilename)
        {
            toplistFilename.ThrowIfNullOrEmpty(nameof(toplistFilename));

            string fileContent = File.ReadAllText(toplistFilename);
            ToplistXml toplistXml = ToplistBase.Desirialize(fileContent);

            ToplistBase toplist = Create(toplistXml.Name, (ToplistType) toplistXml.Type,
                                         (ToplistFormat) toplistXml.Format);
            toplist.UpdateBlocks(toplistXml.ConvertXElementsToBlocks());
            return toplist;
        }
    }
}

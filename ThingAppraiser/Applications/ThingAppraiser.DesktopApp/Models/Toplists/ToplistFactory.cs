using System;
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

            _logger.Info($"Creating toplist [Name: {toplistName}, Type: {toplistType.ToString()} " +
                         $"Format: {toplistFormat.ToString()}].");

            return toplistType switch
            {
                ToplistType.Score => new ScoreToplist(toplistName, toplistFormat),

                ToplistType.Simple => new SimpleToplist(toplistName, toplistFormat),

                _ => throw new ArgumentOutOfRangeException(nameof(toplistType), toplistType,
                                                           "Could not recognize toplist type.")
            };
        }

        public static ToplistBase LoadFromFile(string toplistFilename)
        {
            toplistFilename.ThrowIfNullOrEmpty(nameof(toplistFilename));

            _logger.Info($"Loading toplist from file \"{toplistFilename}\".");

            string fileContent = File.ReadAllText(toplistFilename);
            ToplistXml toplistXml = ToplistBase.Desirialize(fileContent);

            ToplistBase toplist = Create(
                toplistXml.Name, (ToplistType) toplistXml.Type, (ToplistFormat) toplistXml.Format
            );
            toplist.UpdateBlocks(toplistXml.ConvertXElementsToBlocks());
            return toplist;
        }
    }
}

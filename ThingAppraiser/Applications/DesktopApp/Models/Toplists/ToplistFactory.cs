using System;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    internal static class ToplistFactory
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceWithName(nameof(ToplistFactory));

        public static ToplistBase Create(string toplistName, ToplistType toplistType,
            ToplistFormat toplistFormat)
        {
            switch (toplistType)
            {
                case ToplistType.Score:
                    return new ScoreToplist(toplistName, toplistFormat);

                case ToplistType.Simple:
                    return new SimpleToplist(toplistName, toplistFormat);

                default:
                    var ex = new ArgumentOutOfRangeException(nameof(toplistType), toplistType,
                                                             "Could not recognize toplist type.");
                    _logger.Error(ex, "Passed incorrect data to method: " +
                                  $"'{toplistType.ToString()}'.");
                    throw ex;
            }
        }
    }
}

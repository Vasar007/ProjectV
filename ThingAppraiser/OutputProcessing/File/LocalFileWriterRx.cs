using System;
using System.Collections.Generic;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Output
{
    public class CLocalFileWriterRx : IOutputterRx, IOutputterBase, ITagable
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<CLocalFileWriterRx>();

        private readonly LocalFileWriter _localFileWriter = new LocalFileWriter();


        #region ITagable Implementation

        public string Tag { get; } = "LocalFileWriterRx";

        #endregion


        public CLocalFileWriterRx()
        {
        }

        #region IOutputterRx Implementation

        public bool SaveResults(List<List<RatingDataContainer>> results, string storageName)
        {
            if (string.IsNullOrEmpty(storageName)) return false;

            try
            {
                return _localFileWriter.SaveResults(results, storageName);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Couldn't write to the storage.");
                GlobalMessageHandler.OutputMessage($"Couldn't write to the storage. Error: {ex}");
                return false;
            }
        }

        #endregion
    }
}

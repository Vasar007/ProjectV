using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Output
{
    public class CLocalFileWriterAsync : IOutputterAsync, IOutputterBase, ITagable
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<CLocalFileWriterAsync>();

        private readonly LocalFileWriter _localFileWriter = new LocalFileWriter();

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = "LocalFileWriterAsync";

        #endregion


        public CLocalFileWriterAsync()
        {
        }

        #region IOutputterAsync Implementation

        public async Task<bool> SaveResults(List<List<RatingDataContainer>> results,
            string storageName)
        {
            if (string.IsNullOrEmpty(storageName)) return false;

            try
            {
                return await Task.Run(() => _localFileWriter.SaveResults(results, storageName));
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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThingAppraiser.Communication;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.IO.Output.File
{
    public class LocalFileWriterAsync : IOutputterAsync, IOutputterBase, ITagable
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<LocalFileWriterAsync>();

        private readonly LocalFileWriter _localFileWriter = new LocalFileWriter();

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(LocalFileWriterAsync);

        #endregion


        public LocalFileWriterAsync()
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

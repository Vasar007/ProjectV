using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectV.Communication;
using ProjectV.Logging;
using ProjectV.Models.Internal;

namespace ProjectV.IO.Output.File
{
    public sealed class LocalFileWriterAsync : IOutputter, ITagable
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<LocalFileWriterAsync>();

        private readonly LocalFileWriter _localFileWriter;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(LocalFileWriterAsync);

        #endregion


        public LocalFileWriterAsync()
        {
            _localFileWriter = new LocalFileWriter();
        }

        #region IOutputter Implementation

        public async Task<bool> SaveResults(
            IReadOnlyList<IReadOnlyList<RatingDataContainer>> results, string storageName)
        {
            if (string.IsNullOrEmpty(storageName)) return false;

            try
            {
                // Simply wrapping sync file writer with "Task.Run" method.
                return await Task.Run(() => _localFileWriter.SaveResults(results, storageName));
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Could not write to the storage.");
                GlobalMessageHandler.OutputMessage($"Could not write to the storage. Error: {ex}");
                return false;
            }
        }

        #endregion
    }
}

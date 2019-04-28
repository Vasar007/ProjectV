using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Input
{
    public class CLocalFileReaderAsync : IInputterAsync, ITagable
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CLocalFileReaderAsync>();

        private readonly IFileReaderAsync _fileReaderAsync;

        #region ITagable Implementation

        /// <inheritdoc />
        public String Tag => "LocalFileReaderAsync";

        #endregion


        public CLocalFileReaderAsync(IFileReaderAsync fileReaderAsync)
        {
            _fileReaderAsync = fileReaderAsync.ThrowIfNull(nameof(fileReaderAsync));
        }

        #region IInputterAsync Implementation

        public async Task ReadThingNames(BufferBlock<String> queueToWrite, String storageName)
        {
            if (String.IsNullOrEmpty(storageName)) return;

            try
            {
                if (storageName.EndsWith(".csv"))
                {
                    await _fileReaderAsync.ReadCsvFile(queueToWrite, storageName);
                }
                else
                {
                    await _fileReaderAsync.ReadFile(queueToWrite, storageName);
                }
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Async file reader throws exception.");
                throw;
            }

        }

        #endregion
    }
}

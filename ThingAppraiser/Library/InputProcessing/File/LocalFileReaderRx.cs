using System;
using System.Collections.Generic;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Input
{
    public class LocalFileReaderRx : IInputterRx, IInputterBase, ITagable
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<LocalFileReaderRx>();

        private readonly IFileReaderRx _fileReaderRx;

        #region ITagable Implementation

        public string Tag { get; } = "LocalFileReaderRx";

        #endregion


        public LocalFileReaderRx(IFileReaderRx fileReader)
        {
            _fileReaderRx = fileReader.ThrowIfNull(nameof(fileReader));
        }

        #region IInputterRx Implementation

        public IEnumerable<string> ReadThingNames(string storageName)
        {
            storageName.ThrowIfNullOrEmpty(nameof(storageName));

            try
            {
                if (storageName.EndsWith(".csv"))
                {
                    return _fileReaderRx.ReadCsvFile(storageName);
                }
                else
                {
                    return _fileReaderRx.ReadFile(storageName);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Rx file reader throws exception.");
                throw;
            }
        }

        #endregion
    }
}

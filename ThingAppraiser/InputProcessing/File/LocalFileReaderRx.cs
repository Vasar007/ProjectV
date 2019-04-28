using System;
using System.Collections.Generic;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Input
{
    public class CLocalFileReaderRx : IInputterRx, ITagable
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CLocalFileReaderRx>();

        private readonly IFileReaderRx _fileReaderRx;

        #region ITagable Implementation

        public String Tag => "LocalFileReaderRx";

        #endregion


        public CLocalFileReaderRx(IFileReaderRx fileReader)
        {
            _fileReaderRx = fileReader.ThrowIfNull(nameof(fileReader));
        }

        #region IInputterRx Implementation

        public IEnumerable<String> ReadThingNames(String storageName)
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
                s_logger.Error(ex, "Rx file reader throws exception.");
                throw;
            }
        }

        #endregion
    }
}

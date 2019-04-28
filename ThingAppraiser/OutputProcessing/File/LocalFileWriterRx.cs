using System;
using System.Collections.Generic;
using FileHelpers;
using ThingAppraiser.Communication;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Output
{
    public class CLocalFileWriterRx : IOutputterRx, ITagable
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CLocalFileWriterRx>();

        #region ITagable Implementation

        public String Tag => "LocalFileWriterRx";

        #endregion


        public CLocalFileWriterRx()
        {
        }

        private static Boolean WriteFile(IEnumerable<COuputFileData> outputData, String filename)
        {
            if (String.IsNullOrEmpty(filename)) return false;

            var engine = new FileHelperAsyncEngine<COuputFileData>
            {
                HeaderText = typeof(COuputFileData).GetCsvHeader()
            };

            using (engine.BeginWriteFile(filename))
            {
                engine.WriteNexts(outputData);
            }
            return true;
        }

        #region IOutputterRx Implementation

        public Boolean SaveResults(IEnumerable<COuputFileData> outputData, String storageName)
        {
            if (String.IsNullOrEmpty(storageName)) return false;

            try
            {
                return WriteFile(outputData, storageName);
            }
            catch (Exception ex)
            {
                s_logger.Warn(ex, "Couldn't write to the storage.");
                SGlobalMessageHandler.OutputMessage("Couldn't write to the storage. " +
                                                    $"Error: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThingAppraiser.Data;

namespace ThingAppraiser.IO.Output
{
    public class CLocalFileWriterAsync : IOutputterAsync, ITagable
    {
        private readonly CLocalFileWriter _localFileWriter = new CLocalFileWriter();

        #region ITagable Implementation

        /// <inheritdoc />
        public String Tag => "LocalFileWriterAsync";

        #endregion


        public CLocalFileWriterAsync()
        {
        }

        #region IOutputterAsync Implementation

        // Disables because there are no async operations but other outputters may have such ones.
#pragma warning disable 1998
        public async Task<Boolean> SaveResults(List<List<CRatingDataContainer>> results, 
            String storageName)
#pragma warning restore 1998
        {
            return _localFileWriter.SaveResults(results, storageName);
        }

        #endregion
    }
}

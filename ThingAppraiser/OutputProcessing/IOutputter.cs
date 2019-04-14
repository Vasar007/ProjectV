using System;
using System.Collections.Generic;
using ThingAppraiser.Data;

namespace ThingAppraiser.IO.Output
{
    /// <summary>
    /// Defines outputter classes interface to use in output component.
    /// </summary>
    public interface IOutputter : ITagable
    {
        /// <summary>
        /// Saves results of the processing to the storage. It could be NOT only direct writing to
        /// file, you can define your own output sources.
        /// </summary>
        /// <param name="results">Results to save.</param>
        /// <param name="storageName">Storage name in which should be saved results.</param>
        /// <returns>
        /// <c>true</c> if results were saved successfully, <c>false</c> otherwise.
        /// </returns>
        Boolean SaveResults(List<CRating> results, String storageName);
    }
}

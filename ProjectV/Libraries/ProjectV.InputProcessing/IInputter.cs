using System.Collections.Generic;

namespace ProjectV.IO.Input
{
    /// <summary>
    /// Defines inputter classes interface to use in input component.
    /// </summary>
    public interface IInputter : ITagable
    {
        /// <summary>
        /// Reads Things names from storage. It could be NOT only direct reading from file, you can
        /// define your own input sources.
        /// </summary>
        /// <param name="storageName">Storage with Things names.</param>
        /// <returns>Things names as enumeration of strings.</returns>
        IEnumerable<string> ReadThingNames(string storageName);
    }
}

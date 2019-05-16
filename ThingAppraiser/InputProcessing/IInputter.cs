using System.Collections.Generic;

namespace ThingAppraiser.IO.Input
{
    /// <summary>
    /// Defines inputter classes interface to use in input component.
    /// </summary>
    public interface IInputter : IInputterBase, ITagable
    {
        /// <summary>
        /// Reads Things names from storage. It could be NOT only direct reading from file, you can
        /// define your own input sources.
        /// </summary>
        /// <param name="storageName">Storage with Things names.</param>
        /// <returns>Things names as collection of strings.</returns>
        List<string> ReadThingNames(string storageName);
    }
}

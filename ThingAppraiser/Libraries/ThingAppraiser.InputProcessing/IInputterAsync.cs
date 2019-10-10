using System.Collections.Generic;

namespace ThingAppraiser.IO.Input
{
    public interface IInputterAsync : IInputterBase, ITagable
    {
        IEnumerable<string> ReadThingNames(string storageName);
    }
}

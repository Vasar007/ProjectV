using System.Collections.Generic;

namespace ThingAppraiser.IO.Input
{
    public interface IInputterRx : IInputterBase, ITagable
    {
        IEnumerable<string> ReadThingNames(string storageName);
    }
}

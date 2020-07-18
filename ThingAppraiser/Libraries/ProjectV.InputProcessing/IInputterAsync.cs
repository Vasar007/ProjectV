using System.Collections.Generic;

namespace ProjectV.IO.Input
{
    public interface IInputterAsync : IInputterBase, ITagable
    {
        IEnumerable<string> ReadThingNames(string storageName);
    }
}

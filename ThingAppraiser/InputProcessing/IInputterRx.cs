using System;
using System.Collections.Generic;

namespace ThingAppraiser.IO.Input
{
    public interface IInputterRx : ITagable
    {
        IEnumerable<String> ReadThingNames(String storageName);
    }
}

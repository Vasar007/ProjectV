using System.Collections.Generic;

namespace ThingAppraiser.IO.Input
{
    public interface IInputter
    {
        List<string> ReadThingNames(string storageName);
    }
}

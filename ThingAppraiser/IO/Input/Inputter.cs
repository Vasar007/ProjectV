using System.Collections.Generic;

namespace ThingAppraiser.IO.Input
{
    public abstract class Inputter
    {
        public abstract List<string> ReadThingNames(string storageName);
    }
}

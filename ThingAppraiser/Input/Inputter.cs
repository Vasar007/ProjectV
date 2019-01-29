using System;
using System.Collections.Generic;

namespace ThingAppraiser.Input
{
    public abstract class Inputter
    {
        public abstract List<string> ReadNames(string storageName);
    }
}

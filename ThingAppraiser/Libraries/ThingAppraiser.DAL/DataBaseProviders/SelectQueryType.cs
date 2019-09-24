using System;

namespace ThingAppraiser.DAL.DataBaseProviders
{
    [Flags]
    internal enum SelectQueryType
    {
        Min = 0,
        Max = 1,
        MinMax = 2 >> 0
    }
}

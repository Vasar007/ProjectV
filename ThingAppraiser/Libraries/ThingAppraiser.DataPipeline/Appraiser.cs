using System;
using System.Collections.Generic;
using ThingAppraiser.Extensions;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.DataPipeline
{
    public sealed class Appraiser
    {
        public Func<IReadOnlyList<BasicInfo>, IReadOnlyList<string>> Func { get; }

        public Type DataType { get; }


        public Appraiser(Func<IReadOnlyList<BasicInfo>, IReadOnlyList<string>> func, Type dataType)
        {
            Func = func.ThrowIfNull(nameof(func));
            DataType = dataType.ThrowIfNull(nameof(dataType));
        }
    }
}

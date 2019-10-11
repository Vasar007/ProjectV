using System;
using ThingAppraiser.Extensions;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.DataPipeline
{
    public sealed class Funcotype
    {
        public Func<BasicInfo, RatingDataContainer> Func { get; }

        public Type DataType { get; }


        public Funcotype(Func<BasicInfo, RatingDataContainer> func, Type dataType)
        {
            Func = func.ThrowIfNull(nameof(func));
            DataType = dataType.ThrowIfNull(nameof(dataType));
        }
    }
}

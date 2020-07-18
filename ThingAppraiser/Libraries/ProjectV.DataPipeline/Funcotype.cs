using System;
using Acolyte.Assertions;
using ProjectV.Models.Data;
using ProjectV.Models.Internal;

namespace ProjectV.DataPipeline
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

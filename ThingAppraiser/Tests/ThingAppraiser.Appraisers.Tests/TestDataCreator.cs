using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers.Tests
{
    internal static class TestDataCreator
    {
        internal static RawDataContainer CreateRawDataContainerWithBasicInfo(
            params BasicInfo[]? items)
        {
            items ??= new BasicInfo[0];
            var result = new RawDataContainer(new List<BasicInfo>(items));

            if (!result.RawData.Any()) return result;

            result.AddParameter(
                nameof(BasicInfo.VoteCount),
                MinMaxDenominator.CreateForCollection(result.RawData, basic => basic.VoteCount)
            );

            result.AddParameter(
                nameof(BasicInfo.VoteAverage),
                MinMaxDenominator.CreateForCollection(result.RawData, basic => basic.VoteAverage)
            );

            return result;
        }
    }
}

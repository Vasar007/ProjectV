using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Appraisers.Appraisals;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers.Tests
{
    internal static class TestDataCreator
    {
        private static readonly Random RandomInstance = new Random();


        internal static RawDataContainer CreateRawDataContainerWithBasicInfo(
            params BasicInfo[]? items)
        {
            items ??= new BasicInfo[0];

            return CreateRawDataContainerWithBasicInfo(items.AsEnumerable());
        }

        internal static RawDataContainer CreateRawDataContainerWithBasicInfo(
            IEnumerable<BasicInfo> items)
        {
            items.ThrowIfNull(nameof(items));

            var container = new RawDataContainer(new List<BasicInfo>(items));

            if (!container.RawData.Any()) return container;

            container.AddParameter(
                nameof(BasicInfo.VoteCount),
                MinMaxDenominator.CreateForCollection(container.RawData, basic => basic.VoteCount)
            );

            container.AddParameter(
                nameof(BasicInfo.VoteAverage),
                MinMaxDenominator.CreateForCollection(container.RawData, basic => basic.VoteAverage)
            );

            return container;
        }

        internal static IReadOnlyList<ResultInfo> CreateExpectedValueForBasicInfo(Guid ratingId,
            RawDataContainer rawDataContainer, params BasicInfo[] items)
        {
            rawDataContainer.ThrowIfNull(nameof(rawDataContainer));
            items.ThrowIfNull(nameof(items));

            return CreateExpectedValueForBasicInfo(ratingId, rawDataContainer,
                                                   items.AsEnumerable());
        }

        internal static IReadOnlyList<ResultInfo> CreateExpectedValueForBasicInfo(Guid ratingId,
            RawDataContainer rawDataContainer, IEnumerable<BasicInfo> items)
        {
            rawDataContainer.ThrowIfNull(nameof(rawDataContainer));
            items.ThrowIfNull(nameof(items));

            var appraisal = new BasicAppraisal();

            appraisal.PrepareCalculation(rawDataContainer);

            var expectedValue = new List<ResultInfo>();
            foreach (BasicInfo item in items)
            {
                double expectedRating = appraisal.CalculateRating(item);
                var expectedItem = new ResultInfo(item.ThingId, expectedRating, ratingId);

                expectedValue.Add(expectedItem);
            }

            return expectedValue;
        }

        internal static IReadOnlyList<BasicInfo> CreateBasicInfoListRandomly(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count,
                                                      "Count parameter must be positive.");
            }

            return Enumerable
                .Range(1, count)
                .Select(i => new BasicInfo(
                    thingId:     i,
                    title:       $"Title-{i.ToString()}-{CreateRandomString(count, RandomInstance)}",
                    voteCount:   i * RandomInstance.Next(),
                    voteAverage: i * RandomInstance.NextDouble()
                ))
                .ToList();
        }

        private static string CreateRandomString(int length, Random? random = null)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), length,
                                                      "Length must pe positive.");
            }

            random ??= new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(
                Enumerable.Repeat(chars, length)
                    .Select(str => str[random.Next(str.Length)])
                    .ToArray()
            );
        }
    }
}

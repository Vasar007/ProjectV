using System;
using System.Collections.Generic;
using System.Linq;
using Acolyte.Assertions;
using ProjectV.Models.Data;
using ProjectV.Models.Internal;

namespace ProjectV.Appraisers.Tests
{
    internal static class TestDataCreator
    {
        private static Random RandomInstance { get; } = new Random();


        internal static IReadOnlyList<RatingDataContainer> CreateExpectedValueForBasicInfo(
            Guid ratingId, params BasicInfo[] items)
        {
            items.ThrowIfNull(nameof(items));

            return CreateExpectedValueForBasicInfo(ratingId, items.AsEnumerable());
        }

        internal static IReadOnlyList<RatingDataContainer> CreateExpectedValueForBasicInfo(
            Guid ratingId, IEnumerable<BasicInfo> items)
        {
            items.ThrowIfNull(nameof(items));

            var appraisal = TestAppraisersCreator.CreateBasicAppraisal();

            var expectedValue = new List<RatingDataContainer>();
            foreach (BasicInfo item in items)
            {
                double expectedRating = appraisal.CalculateRating(item);
                var expectedItem = new RatingDataContainer(item, expectedRating, ratingId);

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
                    thingId: i,
                    title: $"Title-{i.ToString()}-{CreateRandomString(count, RandomInstance)}",
                    voteCount: i * RandomInstance.Next(),
                    voteAverage: i * RandomInstance.NextDouble()
                ))
                .ToList();
        }

        private static string CreateRandomString(int length, Random? random = null)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), length,
                                                      "Length must be positive.");
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

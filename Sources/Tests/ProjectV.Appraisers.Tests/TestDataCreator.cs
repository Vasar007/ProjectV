using System;
using System.Collections.Generic;
using System.Linq;
using Acolyte.Assertions;
using ProjectV.Models.Data;
using ProjectV.Models.Internal;
using ProjectV.Tests.Shared.Helpers.Generators.Models;

namespace ProjectV.Appraisers.Tests
{
    /// <summary>
    /// Legacy static creators for appraiser test data.
    /// </summary>
    /// <remarks>
    /// <see cref="CreateExpectedValueForBasicInfo(Guid, BasicInfo[])" />
    /// computes the expected rating by running the same production
    /// <c>BasicAppraisalCommon.CalculateRating</c> the SUT delegates to, so
    /// tests comparing against it verify delegation/wiring only — a wrong
    /// rating formula cannot fail them. Formula regressions are covered
    /// separately by a test with a hand-computed expected value
    /// (see <c>AppraiserTests.GetRatings_WithKnownVotes_ReturnsVoteAverageAsRatingValue</c>).
    /// </remarks>
    internal static class TestDataCreator
    {
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

            // Sequential thing ids keep the elements addressable; the
            // generator fills the remaining fields with random values that
            // respect the BasicInfo domain (unique GUID-suffixed titles,
            // vote counts in [10, 10_000), vote averages in [0.0, 10.0]).
            return Enumerable
                .Range(1, count)
                .Select(i => BasicInfoGenerator.Instance.GenerateBasicInfo(thingId: i))
                .ToList();
        }
    }
}

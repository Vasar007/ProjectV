using ThingAppraiser.Appraisers.Appraisals;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Appraisers.Tests
{
    internal static class TestAppraisersCreator
    {
        public static IAppraiser CreateTmdbNormalizedAppraiser()
        {
            var appraisal = new BasicAppraisal();

            return new Appraiser<BasicInfo>(appraisal);
        }
    }
}

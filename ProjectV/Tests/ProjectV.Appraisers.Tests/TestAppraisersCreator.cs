using ProjectV.Appraisers.Appraisals;
using ProjectV.Models.Data;

namespace ProjectV.Appraisers.Tests
{
    internal static class TestAppraisersCreator
    {
        public static IAppraiser CreateBasicAppraiser()
        {
            var appraisal = new BasicAppraisal();

            return new Appraiser<BasicInfo>(appraisal);
        }
    }
}

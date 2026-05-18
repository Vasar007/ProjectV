using ProjectV.Appraisers.Appraisals;
using ProjectV.Models.Data;

namespace ProjectV.Appraisers.Tests
{
    internal static class TestAppraisersCreator
    {
        public static IAppraisal<BasicInfo> CreateBasicAppraisal()
        {
            return new BasicAppraisalCommon();
        }

        public static IAppraiser CreateBasicAppraiser()
        {
            var appraisal = CreateBasicAppraisal();
            return new Appraiser<BasicInfo>(appraisal);
        }
    }
}

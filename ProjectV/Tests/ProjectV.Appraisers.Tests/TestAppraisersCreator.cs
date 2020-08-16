using ProjectV.Appraisers.Appraisals;
using ProjectV.Models.Data;

namespace ProjectV.Appraisers.Tests
{
    internal static class TestAppraisersCreator
    {
        public static IAppraisal<BasicInfo> CreateBasicAppraisal()
        {
            return new BasicAppraisal();
        }

        public static IAppraiserAsync CreateBasicAppraiser()
        {
            return new AppraiserAsync<BasicInfo>(CreateBasicAppraisal());
        }
    }
}

using ThingAppraiser.Models.WebService;

namespace ThingAppraiser.Building
{
    public interface IRequirementsCreator
    {
        void Reset();

        void AddInputRequirement(string inputRequirement);

        void AddServiceRequirement(string serviceRequirement);

        void AddAppraisalRequirement(string appraisalRequirement);

        void AddOutputRequirement(string outputRequirement);

        ConfigRequirements GetResult();
    }
}

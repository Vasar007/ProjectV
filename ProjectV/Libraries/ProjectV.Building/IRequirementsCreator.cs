using ProjectV.Models.WebService.Requests;

namespace ProjectV.Building
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

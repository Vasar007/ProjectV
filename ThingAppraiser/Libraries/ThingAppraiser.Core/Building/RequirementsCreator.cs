using System.Collections.Generic;
using ThingAppraiser.Models.WebService;

namespace ThingAppraiser.Core.Building
{
    public sealed class RequirementsCreator : IRequirementsCreator
    {
        // Initializes in method call in ctor.
        private ConfigRequirements _configRequirements = default!;


        public RequirementsCreator()
        {
            Reset();
        }

        public void Reset()
        {
            _configRequirements = new ConfigRequirements
            {
                Input = new List<string>(),
                Services = new List<string>(),
                Appraisals = new List<string>(),
                Output = new List<string>()
            };
        }

        public void AddInputRequirement(string inputRequirement)
        {
            ConfigContract.CheckAvailability(inputRequirement, ConfigContract.AvailableInput);

            _configRequirements.Input.Add(inputRequirement);
        }

        public void AddServiceRequirement(string serviceRequirement)
        {
            ConfigContract.CheckAvailability(serviceRequirement,
                                             ConfigContract.AvailableServices);

            _configRequirements.Services.Add(serviceRequirement);
        }

        public void AddAppraisalRequirement(string appraisalRequirement)
        {
            ConfigContract.CheckAvailability(appraisalRequirement,
                                             ConfigContract.AvailableAppraisals);

            _configRequirements.Appraisals.Add(appraisalRequirement);
        }

        public void AddOutputRequirement(string outputRequirement)
        {
            ConfigContract.CheckAvailability(outputRequirement, ConfigContract.AvailableOutput);

            _configRequirements.Output.Add(outputRequirement);
        }

        public ConfigRequirements GetResult()
        {
            // Warning! You should avoid errors with further editing.
            return _configRequirements;
        }
    }
}

using System.Collections.Generic;
using ThingAppraiser.Data.Models;

namespace ThingAppraiser.Core.Building
{
    public class RequirementsCreator : IRequirementsCreator
    {
        private ConfigRequirements _configRequirements;


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
            // Return current value and reset reference to avoid errors with further editing.
            ConfigRequirements result = _configRequirements;
            Reset();
            return result;
        }
    }
}

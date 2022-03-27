using System.Collections.Generic;
using ProjectV.Configuration;
using ProjectV.Models.WebService;

namespace ProjectV.Building
{
    public sealed class RequirementsCreator : IRequirementsCreator
    {
        private readonly List<string> _input = new();

        private readonly List<string> _services = new();

        private readonly List<string> _appraisals = new();

        private readonly List<string> _output = new();


        public RequirementsCreator()
        {
            Reset();
        }

        #region IRequirementsCreator Implementation

        public void Reset()
        {
            _input.Clear();
            _services.Clear();
            _appraisals.Clear();
            _output.Clear();
        }

        public void AddInputRequirement(string inputRequirement)
        {
            ConfigContract.CheckAvailability(inputRequirement, ConfigContract.AvailableInput);

            _input.Add(inputRequirement);
        }

        public void AddServiceRequirement(string serviceRequirement)
        {
            ConfigContract.CheckAvailability(serviceRequirement,
                                             ConfigContract.AvailableServices);

            _services.Add(serviceRequirement);
        }

        public void AddAppraisalRequirement(string appraisalRequirement)
        {
            ConfigContract.CheckAvailability(appraisalRequirement,
                                             ConfigContract.AvailableAppraisals);

            _appraisals.Add(appraisalRequirement);
        }

        public void AddOutputRequirement(string outputRequirement)
        {
            ConfigContract.CheckAvailability(outputRequirement, ConfigContract.AvailableOutput);

            _output.Add(outputRequirement);
        }

        public ConfigRequirements GetResult()
        {
            return new ConfigRequirements(
                input: _input.ToArray(),
                services: _services.ToArray(),
                appraisals: _appraisals.ToArray(),
                output: _output.ToArray()
            );
        }

        #endregion
    }
}

using System.Collections.Generic;
using Acolyte.Assertions;
using Newtonsoft.Json;

namespace ProjectV.Models.WebService.Requests
{
    public sealed class ConfigRequirements
    {
        public IReadOnlyList<string> Input { get; }

        public IReadOnlyList<string> Services { get; }

        public IReadOnlyList<string> Appraisals { get; }

        public IReadOnlyList<string> Output { get; }


        [JsonConstructor]
        public ConfigRequirements(
            IReadOnlyList<string> input,
            IReadOnlyList<string> services,
            IReadOnlyList<string> appraisals,
            IReadOnlyList<string> output)
        {
            Input = input.ThrowIfNull(nameof(input));
            Services = services.ThrowIfNull(nameof(services));
            Appraisals = appraisals.ThrowIfNull(nameof(appraisals));
            Output = output.ThrowIfNull(nameof(output));
        }
    }
}

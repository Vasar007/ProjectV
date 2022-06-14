using System.ComponentModel.DataAnnotations;
using Acolyte.Assertions;

namespace ProjectV.Configuration.Options
{
    public sealed class RestApiOptions : IOptions
    {
        [Required(AllowEmptyStrings = false)]
        public string CommunicationServiceBaseAddress { get; init; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string CommunicationServiceRequestApiUrl { get; init; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string CommunicationServiceLoginApiUrl { get; init; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string ConfigurationServiceBaseAddress { get; init; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string ConfigurationServiceApiUrl { get; init; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string ProcessingServiceBaseAddress { get; init; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string ProcessingServiceApiUrl { get; init; } = default!;


        public RestApiOptions()
        {
        }

        #region IOptions Implementation

        public void Validate()
        {
            CommunicationServiceBaseAddress.ThrowIfNullOrWhiteSpace(nameof(CommunicationServiceBaseAddress));
            CommunicationServiceRequestApiUrl.ThrowIfNullOrWhiteSpace(nameof(CommunicationServiceRequestApiUrl));
            CommunicationServiceLoginApiUrl.ThrowIfNullOrWhiteSpace(nameof(CommunicationServiceLoginApiUrl));

            ConfigurationServiceBaseAddress.ThrowIfNullOrWhiteSpace(nameof(ConfigurationServiceBaseAddress));
            ConfigurationServiceApiUrl.ThrowIfNullOrWhiteSpace(nameof(ConfigurationServiceApiUrl));

            ProcessingServiceBaseAddress.ThrowIfNullOrWhiteSpace(nameof(ProcessingServiceBaseAddress));
            ProcessingServiceApiUrl.ThrowIfNullOrWhiteSpace(nameof(ProcessingServiceApiUrl));
        }

        #endregion
    }
}

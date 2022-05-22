using System.ComponentModel.DataAnnotations;

namespace ProjectV.Configuration.Options
{
    public sealed class RestApiOptions : IOptions
    {
        [Required(AllowEmptyStrings = false)]
        public string CommunicationServiceBaseAddress { get; set; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string CommunicationServiceRequestApiUrl { get; set; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string CommunicationServiceLoginApiUrl { get; set; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string ConfigurationServiceBaseAddress { get; set; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string ConfigurationServiceApiUrl { get; set; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string ProcessingServiceBaseAddress { get; set; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string ProcessingServiceApiUrl { get; set; } = default!;


        public RestApiOptions()
        {
        }
    }
}

namespace ProjectV.Configuration.Options
{
    public sealed class RestApiOptions : IOptions
    {
        public string CommunicationServiceBaseAddress { get; set; } = default!;

        public string CommunicationServiceRequestApiUrl { get; set; } = default!;

        public string CommunicationServiceLoginApiUrl { get; set; } = default!;

        public string ConfigurationServiceBaseAddress { get; set; } = default!;

        public string ConfigurationServiceApiUrl { get; set; } = default!;

        public string ProcessingServiceBaseAddress { get; set; } = default!;

        public string ProcessingServiceApiUrl { get; set; } = default!;


        public RestApiOptions()
        {
        }
    }
}

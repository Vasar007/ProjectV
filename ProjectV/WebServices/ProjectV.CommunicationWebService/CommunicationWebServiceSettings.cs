namespace ProjectV.CommunicationWebService
{
    // TODO: make this DTO immutable.
    public sealed class CommunicationWebServiceSettings
    {
        public string ConfigurationServiceBaseAddress { get; set; } = default!;

        public string ConfigurationServiceApiUrl { get; set; } = default!;

        public string ProcessingServiceBaseAddress { get; set; } = default!;

        public string ProcessingServiceApiUrl { get; set; } = default!;


        public CommunicationWebServiceSettings()
        {
        }
    }
}

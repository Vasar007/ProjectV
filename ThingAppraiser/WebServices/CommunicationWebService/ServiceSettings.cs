namespace ThingAppraiser.CommunicationWebService
{
    public sealed class ServiceSettings
    {
        public string ConfigurationServiceBaseAddress { get; set; }

        public string ConfigurationServiceApiUrl { get; set; }

        public string ProcessingServiceBaseAddress { get; set; }

        public string ProcessingServiceApiUrl { get; set; }


        public ServiceSettings()
        {
        }
    }
}
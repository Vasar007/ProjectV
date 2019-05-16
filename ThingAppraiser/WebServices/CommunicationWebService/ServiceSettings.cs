namespace ThingAppraiser.CommunicationWebService
{
    public class ServiceSettings
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
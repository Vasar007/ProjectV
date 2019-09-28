namespace ThingAppraiser.Configuration
{
    public sealed class ThingAppraiserServiceOptions : IOptions
    {
        public string CommunicationServiceBaseAddress { get; set; } = "https://localhost:44359/";

        public string CommunicationServiceApiUrl { get; set; } = "api/v1/requests";


        public ThingAppraiserServiceOptions()
        {
        }
    }
}

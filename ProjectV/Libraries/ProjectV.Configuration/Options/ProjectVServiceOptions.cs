namespace ProjectV.Configuration.Options
{
    public sealed class ProjectVServiceOptions : IOptions
    {
        public string CommunicationServiceBaseAddress { get; set; } = "https://localhost:44322/";

        public string CommunicationServiceApiUrl { get; set; } = "api/v1/requests";


        public ProjectVServiceOptions()
        {
        }
    }
}

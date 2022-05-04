namespace ProjectV.Configuration.Options
{
    public sealed class ProjectVServiceOptions : IOptions
    {
        public string CommunicationServiceBaseAddress { get; set; } = default!;

        public string CommunicationServiceRequestApiUrl { get; set; } = default!;

        public string CommunicationServiceLoginApiUrl { get; set; } = default!;

        public string? AccessToken { get; set; } =
            EnvironmentVariablesParser.GetValueOrDefault("AccessToken", string.Empty);


        public ProjectVServiceOptions()
        {
        }
    }
}

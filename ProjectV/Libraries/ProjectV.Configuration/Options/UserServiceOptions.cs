namespace ProjectV.Configuration.Options
{
    // TODO: make this DTO immutable.
    public sealed class UserServiceOptions : IOptions
    {
        public bool AllowSignup { get; set; }

        public bool ShouldCreateSystemUser { get; set; }

        public string? SystemUserName { get; set; } =
            EnvironmentVariablesParser.GetValueOrDefault("SystemUserName", string.Empty);

        public string? SystemUserPassword { get; set; } =
            EnvironmentVariablesParser.GetValueOrDefault("SystemUserPassword", string.Empty);


        public UserServiceOptions()
        {
        }
    }
}

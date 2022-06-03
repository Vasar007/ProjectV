namespace ProjectV.Configuration.Options
{
    public sealed class UserServiceOptions : IOptions
    {
        public bool AllowSignup { get; set; }

        public bool ShouldCreateSystemUser { get; set; } = false;

        public bool CanUseSystemUserToAuthenticate { get; set; } = false;

        public string? SystemUserName { get; set; } =
            EnvironmentVariablesParser.GetValueOrDefault("SystemUserName", string.Empty);

        public string? SystemUserPassword { get; set; } =
            EnvironmentVariablesParser.GetValueOrDefault("SystemUserPassword", string.Empty);


        public UserServiceOptions()
        {
        }
    }
}

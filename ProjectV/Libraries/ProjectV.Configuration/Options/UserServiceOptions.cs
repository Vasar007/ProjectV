using Acolyte.Assertions;

namespace ProjectV.Configuration.Options
{
    public sealed class UserServiceOptions : IOptions
    {
        public bool AllowSignup { get; init; }

        public bool ShouldCreateSystemUser { get; init; } = false;

        public bool CanUseSystemUserToAuthenticate { get; init; } = false;

        public string? SystemUserName { get; init; } =
            EnvironmentVariablesParser.GetValueOrDefault("SystemUserName", string.Empty);

        public string? SystemUserPassword { get; init; } =
            EnvironmentVariablesParser.GetValueOrDefault("SystemUserPassword", string.Empty);


        public UserServiceOptions()
        {
        }

        #region IOptions Implementation

        public void Validate()
        {
            if (ShouldCreateSystemUser || CanUseSystemUserToAuthenticate)
            {
                SystemUserName.ThrowIfNullOrWhiteSpace(nameof(SystemUserName));
                SystemUserPassword.ThrowIfNullOrWhiteSpace(nameof(SystemUserPassword));
            }
        }

        #endregion
    }
}

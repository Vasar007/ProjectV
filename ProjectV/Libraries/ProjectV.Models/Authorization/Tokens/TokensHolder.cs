namespace ProjectV.Models.Authorization.Tokens
{
    public sealed record TokensHolder(
        string AccessToken,
        string RefreshToken
    );
}

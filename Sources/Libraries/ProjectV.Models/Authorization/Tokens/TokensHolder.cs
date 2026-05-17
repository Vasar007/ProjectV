namespace ProjectV.Models.Authorization.Tokens
{
    public sealed record TokensHolder(
        AccessTokenData AccessToken,
        RefreshTokenData RefreshToken
    );
}

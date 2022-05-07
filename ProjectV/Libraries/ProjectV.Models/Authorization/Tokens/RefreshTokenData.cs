using System;

namespace ProjectV.Models.Authorization.Tokens
{
    public sealed record RefreshTokenData : TokenData
    {
        public RefreshTokenData(
            string Token,
            DateTime ExpiryDateUtc)
            : base(Token, ExpiryDateUtc)
        {
        }
    }
}

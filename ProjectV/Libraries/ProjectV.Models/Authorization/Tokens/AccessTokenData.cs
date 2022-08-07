using System;

namespace ProjectV.Models.Authorization.Tokens
{
    public sealed record AccessTokenData : TokenData
    {
        public AccessTokenData(
            string Token,
            DateTime ExpiryDateUtc)
            : base(Token, ExpiryDateUtc)
        {
        }
    }
}

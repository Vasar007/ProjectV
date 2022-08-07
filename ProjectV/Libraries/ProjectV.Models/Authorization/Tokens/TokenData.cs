using System;

namespace ProjectV.Models.Authorization.Tokens
{
    public record TokenData(
        string Token,
        DateTime ExpiryDateUtc
    );
}

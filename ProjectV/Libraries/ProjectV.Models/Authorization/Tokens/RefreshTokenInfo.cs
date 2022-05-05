using System;
using Acolyte.Assertions;
using ProjectV.Models.Basic;
using ProjectV.Models.Users;

namespace ProjectV.Models.Authorization.Tokens
{
    public sealed class RefreshTokenInfo : IEntity<RefreshTokenId>
    {
        public RefreshTokenId Id { get; }
        public UserId UserId { get; }
        public Password TokenHash { get; }
        public string TokenSalt { get; }
        public DateTime TimestampUtc { get; }
        public DateTime ExpiryDateUtc { get; }

        public RefreshTokenInfo(
            RefreshTokenId id,
            UserId userId,
            Password tokenHash,
            string tokenSalt,
            DateTime timestampUtc,
            DateTime expiryDateUtc)
        {
            Id = id;
            UserId = userId;
            TokenHash = tokenHash;
            TokenSalt = tokenSalt.ThrowIfNull(nameof(tokenSalt));
            TimestampUtc = timestampUtc;
            ExpiryDateUtc = expiryDateUtc;
        }
    }
}

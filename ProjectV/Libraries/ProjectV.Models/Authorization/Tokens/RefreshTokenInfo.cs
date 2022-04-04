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
        public string TokenHash { get; }
        public string TokenSalt { get; }
        public DateTime Ts { get; }
        public DateTime ExpiryDate { get; }

        public RefreshTokenInfo(
            RefreshTokenId id,
            UserId userId,
            string tokenHash,
            string tokenSalt,
            DateTime ts,
            DateTime expiryDate)
        {
            Id = id;
            UserId = userId;
            TokenHash = tokenHash.ThrowIfNull(nameof(tokenHash));
            TokenSalt = tokenSalt.ThrowIfNull(nameof(tokenSalt));
            Ts = ts;
            ExpiryDate = expiryDate;
        }
    }
}

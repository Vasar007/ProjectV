using System;
using Acolyte.Assertions;
using Acolyte.Common;
using ProjectV.Models.Basic;
using ProjectV.Models.Users;

namespace ProjectV.Models.Authorization.Tokens
{
    public sealed class RefreshTokenInfo : IEntity<RefreshTokenId>, IHaveCreationTime
    {
        public RefreshTokenId Id { get; }
        public UserId UserId { get; }
        public Password TokenHash { get; }
        public string TokenSalt { get; }
        public DateTime CreationTimeUtc { get; }
        public DateTime ExpiryDateUtc { get; }


        public RefreshTokenInfo(
            RefreshTokenId id,
            UserId userId,
            Password tokenHash,
            string tokenSalt,
            DateTime creationTimeUtc,
            DateTime expiryDateUtc)
        {
            Id = id;
            UserId = userId;
            TokenHash = tokenHash;
            TokenSalt = tokenSalt.ThrowIfNull(nameof(tokenSalt));
            CreationTimeUtc = creationTimeUtc;
            ExpiryDateUtc = expiryDateUtc;
        }
    }
}

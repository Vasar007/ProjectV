using System;
using Acolyte.Assertions;
using Acolyte.Common;
using ProjectV.Models.Authorization;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.Basic;

namespace ProjectV.Models.Users
{
    public sealed class UserInfo : IEntity<UserId>, IHaveCreationTime
    {
        public UserId Id { get; }
        public UserName UserName { get; }
        public Password Password { get; }
        public string PasswordSalt { get; }
        public DateTime CreationTimeUtc { get; }
        public bool Active { get; }

        public RefreshTokenInfo? RefreshToken { get; }
        //public ICollection<Task>? Tasks { get; }


        public UserInfo(
            UserId id,
            UserName userName,
            Password password,
            string passwordSalt,
            DateTime creationTimeUtc,
            bool active,
            RefreshTokenInfo? refreshToken)
        {
            Id = id;
            UserName = userName;
            Password = password;
            PasswordSalt = passwordSalt.ThrowIfNullOrWhiteSpace(nameof(passwordSalt));
            CreationTimeUtc = creationTimeUtc;
            Active = active;
            RefreshToken = refreshToken;
        }
    }
}

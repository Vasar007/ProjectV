using System;
using Acolyte.Assertions;
using ProjectV.Models.Authorization;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.Basic;

namespace ProjectV.Models.Users
{
    public sealed class UserInfo : IEntity<UserId>
    {
        public UserId Id { get; }
        public UserName UserName { get; }
        public Password Password { get; }
        public string PasswordSalt { get; }
        public DateTime TimestampUtc { get; }
        public bool Active { get; }

        public RefreshTokenInfo? RefreshToken { get; }
        //public ICollection<Task>? Tasks { get; }


        public UserInfo(
            UserId id,
            UserName userName,
            Password password,
            string passwordSalt,
            DateTime timestampUtc,
            bool active,
            RefreshTokenInfo? refreshToken)
        {
            Id = id;
            UserName = userName;
            Password = password;
            PasswordSalt = passwordSalt.ThrowIfNullOrWhiteSpace(nameof(passwordSalt));
            TimestampUtc = timestampUtc;
            Active = active;
            RefreshToken = refreshToken;
        }
    }
}

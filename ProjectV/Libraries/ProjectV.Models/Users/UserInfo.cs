using System;
using Acolyte.Assertions;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.Basic;

namespace ProjectV.Models.Users
{
    public sealed class UserInfo : IEntity<UserId>
    {
        public UserId Id { get; }
        public UserName UserName { get; }
        public string Password { get; }
        public string PasswordSalt { get; }
        public DateTime Ts { get; }
        public bool Active { get; }

        public RefreshTokenInfo? RefreshToken { get; }
        //public ICollection<Task>? Tasks { get; }


        public UserInfo(
            UserId id,
            UserName userName,
            string password,
            string passwordSalt,
            DateTime ts,
            bool active,
            RefreshTokenInfo? refreshToken)
        {
            Id = id;
            UserName = userName;
            Password = password.ThrowIfNullOrWhiteSpace(nameof(password));
            PasswordSalt = passwordSalt.ThrowIfNullOrWhiteSpace(nameof(passwordSalt));
            Ts = ts;
            Active = active;
            RefreshToken = refreshToken;
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Acolyte.Assertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectV.DataAccessLayer.Services.Tokens.Models;
using ProjectV.Models.Users;

namespace ProjectV.DataAccessLayer.Services.Users.Models
{
    [Table("users")]
    public sealed class UserDbInfo
    {
        [Key, Required]
        [Column("id")]
        internal Guid Id { get; }
        public UserId WrappedId => UserId.Wrap(Id);

        [Required]
        [Column("user_name")]
        internal string UserName { get; }
        public UserName WrappedUserName => ProjectV.Models.Users.UserName.Wrap(UserName);

        [Required]
        [Column("password")]
        public string Password { get; }

        [Required]
        [Column("password_salt")]
        public string PasswordSalt { get; }

        [Required]
        [Column("ts")]
        public DateTime Ts { get; }

        [Required]
        [Column("active")]
        public bool Active { get; }

        public RefreshTokenDbInfo? RefreshToken { get; }

        //public ICollection<Task>? Tasks { get; }


        public UserDbInfo(
            Guid id,
            string userName,
            string password,
            string passwordSalt,
            DateTime ts,
            bool active,
            RefreshTokenDbInfo? refreshToken)
        {
            Id = id.ThrowIfEmpty(nameof(id));
            UserName = userName.ThrowIfNullOrWhiteSpace(nameof(userName));
            Password = password.ThrowIfNullOrWhiteSpace(nameof(userName));
            PasswordSalt = passwordSalt.ThrowIfNullOrWhiteSpace(nameof(userName));
            Ts = ts;
            Active = active;
            RefreshToken = refreshToken;
        }
    }

    public sealed class UserDbInfoConfiguration : IEntityTypeConfiguration<UserDbInfo>
    {
        public UserDbInfoConfiguration()
        {
        }

        #region IEntityTypeConfiguration<UserDbInfo> Implementation

        public void Configure(EntityTypeBuilder<UserDbInfo> builder)
        {
            // Key fields have already mapped. Need to set the other fields.
            builder.HasKey(e => e.Id);
            builder.Property(e => e.UserName);
            builder.Property(e => e.Password);
            builder.Property(e => e.PasswordSalt);
            builder.Property(e => e.Ts);
            builder.Property(e => e.Active);
            builder.Property(e => e.RefreshToken);
        }

        #endregion
    }
}

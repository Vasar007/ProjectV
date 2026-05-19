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

        /// <summary>
        /// Out-of-band navigation surface for <see cref="DataAccessLayerMapper" />.
        /// Marked <see cref="NotMappedAttribute" /> because the live refresh
        /// token row lives in the separate <c>tokens</c> table (configured by
        /// <see cref="RefreshTokenDbInfoConfiguration" /> via
        /// <c>RefreshTokenDbInfo.UserId</c>); EF Core cannot map a navigation
        /// type through this immutable property and the previous
        /// <c>builder.Property(e =&gt; e.RefreshToken)</c> mapping blocked
        /// model validation. The mapper hydrates this property out-of-band
        /// when needed. See Plan 02-09 Task 1 (Rule 1 fix unblocking
        /// RESEARCH.md Critical Finding #1).
        /// </summary>
        [NotMapped]
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
            : this(id, userName, password, passwordSalt, ts, active)
        {
            RefreshToken = refreshToken;
        }

        // EF Core constructor (no navigation parameter). EF picks the ctor
        // whose every argument binds to a mapped scalar; the 7-arg ctor above
        // cannot be bound because [NotMapped] excludes RefreshToken from the
        // model. This 6-arg overload is the EF-friendly path; production
        // callers continue to use the 7-arg ctor via DataAccessLayerMapper.
        internal UserDbInfo(
            Guid id,
            string userName,
            string password,
            string passwordSalt,
            DateTime ts,
            bool active)
        {
            Id = id.ThrowIfEmpty(nameof(id));
            UserName = userName.ThrowIfNullOrWhiteSpace(nameof(userName));
            Password = password.ThrowIfNullOrWhiteSpace(nameof(password));
            PasswordSalt = passwordSalt.ThrowIfNullOrWhiteSpace(nameof(passwordSalt));
            Ts = ts;
            Active = active;
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
            // RefreshToken is [NotMapped] on the entity — see the property
            // remark on UserDbInfo. The live refresh token row lives in the
            // separate `tokens` table.
        }

        #endregion
    }
}

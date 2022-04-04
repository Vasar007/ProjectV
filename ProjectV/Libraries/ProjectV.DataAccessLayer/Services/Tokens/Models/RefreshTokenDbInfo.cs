using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Acolyte.Assertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.Users;

namespace ProjectV.DataAccessLayer.Services.Tokens.Models
{
    [Table("tokens")]
    public sealed class RefreshTokenDbInfo
    {
        [Key, Required]
        [Column("id")]
        internal Guid Id { get; }
        public RefreshTokenId WrappedId => RefreshTokenId.Wrap(Id);

        [Required]
        [Column("user_name")]
        internal Guid UserId { get; }
        public UserId WrappedUserId { get; }

        [Required]
        [Column("token_hash")]
        public string TokenHash { get; }

        [Required]
        [Column("token_salt")]
        public string TokenSalt { get; }

        [Required]
        [Column("ts")]
        public DateTime Ts { get; }

        [Required]
        [Column("expiry_date")]
        public DateTime ExpiryDate { get; }


        public RefreshTokenDbInfo(
            Guid id,
            Guid userId,
            string tokenHash,
            string tokenSalt,
            DateTime ts,
            DateTime expiryDate)
        {
            Id = id.ThrowIfEmpty(nameof(id));
            UserId = userId;
            TokenHash = tokenHash.ThrowIfNullOrWhiteSpace(nameof(tokenHash));
            TokenSalt = tokenSalt.ThrowIfNullOrWhiteSpace(nameof(tokenSalt));
            Ts = ts;
            ExpiryDate = expiryDate;
        }
    }

    public sealed class RefreshTokenDbInfoConfiguration : IEntityTypeConfiguration<RefreshTokenDbInfo>
    {
        public RefreshTokenDbInfoConfiguration()
        {
        }

        #region IEntityTypeConfiguration<RefreshTokenDbInfo> Implementation

        public void Configure(EntityTypeBuilder<RefreshTokenDbInfo> builder)
        {
            // Key fields have already mapped. Need to set the other fields.
            builder.HasKey(e => e.Id);
            builder.Property(e => e.UserId);
            builder.Property(e => e.TokenHash);
            builder.Property(e => e.TokenSalt);
            builder.Property(e => e.Ts);
            builder.Property(e => e.ExpiryDate);
        }

        #endregion
    }
}

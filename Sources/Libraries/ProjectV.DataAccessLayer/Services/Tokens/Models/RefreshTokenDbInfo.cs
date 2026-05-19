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

        // The Guid property `UserId` above shadows the `UserId` value-object
        // type for name lookup inside this class. Use the fully qualified
        // namespace because the unqualified `Users` token resolves to the
        // sibling DAL namespace `ProjectV.DataAccessLayer.Services.Users`
        // here, not to `ProjectV.Models.Users`.
        public UserId WrappedUserId => ProjectV.Models.Users.UserId.Wrap(UserId);

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


        // EF Core 10 uses this constructor for entity materialization via
        // parameter-name matching against mapped scalar columns. The
        // `ThrowIfEmpty` and `ThrowIfNullOrWhiteSpace` guards therefore fire
        // BOTH on writes (domain code constructing a new token) AND on reads
        // (EF materializing a row from the `tokens` table). A row with
        // `id = Guid.Empty`, `user_name = Guid.Empty`, or null/whitespace
        // hash/salt will throw at query-execution time rather than being
        // returned as a domain object — intentional defense-in-depth that
        // matches the production token-issuance invariants (every token has a
        // real owner). Data-repair scenarios that need to read corrupt rows
        // must fix the SQL first; service-layer queries cannot bypass.
        public RefreshTokenDbInfo(
            Guid id,
            Guid userId,
            string tokenHash,
            string tokenSalt,
            DateTime ts,
            DateTime expiryDate)
        {
            Id = id.ThrowIfEmpty(nameof(id));
            UserId = userId.ThrowIfEmpty(nameof(userId));
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

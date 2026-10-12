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


        // EF Core uses this constructor for entity materialization via
        // parameter-name matching against mapped property names (case-
        // insensitive) — not column names. The `ThrowIfEmpty` and
        // `ThrowIfNullOrWhiteSpace` guards therefore fire both on writes
        // (domain code constructing a new token) and on reads (EF
        // materializing a row from the `tokens` table). A row with
        // `Id = Guid.Empty`, `UserId = Guid.Empty`, or null/whitespace
        // `TokenHash`/`TokenSalt` will throw at query-execution time rather
        // than being returned as a domain object — this ctor is the
        // EF-materialization-path enforcement of those invariants (no
        // DB-level CHECK constraint exists; the parallel
        // `RefreshTokenInfo` domain model carries its own value-object and
        // null guards on the write path). `Ts` and `ExpiryDate` carry no
        // ctor-level guard; temporal invariants (e.g. `ExpiryDate < Ts`)
        // are not checked here.
        //
        // Operationally: service-layer reads and deletes route through
        // `FindByIdAsync` (the latter via `DeleteAsync`), so a corrupt row
        // in the DB cannot be read or deleted through the service. Updates
        // are protected by the domain-layer input `UpdateAsync` receives
        // (not by a fetch), so a corrupt-in-DB row likewise cannot be
        // overwritten by a service call without first being read.
        // Data-repair scenarios that need to touch such rows must fix
        // them in SQL first.
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

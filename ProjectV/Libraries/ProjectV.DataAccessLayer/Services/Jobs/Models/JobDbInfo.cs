using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Acolyte.Assertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectV.Models.Internal.Jobs;

namespace ProjectV.DataAccessLayer.Services.Jobs.Models
{
    [Table("jobs")]
    public sealed class JobDbInfo
    {
        [Key, Required]
        [Column("id")]
        internal Guid Id { get; }
        public JobId WrappedId => JobId.Wrap(Id);

        [Required]
        [Column("name")]
        public string Name { get; }

        [Required]
        [Column("state")]
        public int State { get; }

        [Required]
        [Column("result")]
        public int Result { get; }

        [Required]
        [Column("config")]
        public string Config { get; }


        public JobDbInfo(
            Guid id,
            string name,
            int state,
            int result,
            string config)
        {
            Id = id.ThrowIfEmpty(nameof(id));
            Name = name.ThrowIfNullOrWhiteSpace(nameof(config));
            State = state;
            Result = result;
            Config = config.ThrowIfNullOrWhiteSpace(nameof(config));
        }
    }

    public sealed class JobDbInfoConfiguration : IEntityTypeConfiguration<JobDbInfo>
    {
        public JobDbInfoConfiguration()
        {
        }

        #region IEntityTypeConfiguration<JobDbInfo> Implementation

        public void Configure(EntityTypeBuilder<JobDbInfo> builder)
        {
            // Key fields have already mapped. Need to set the other fields.
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name);
            builder.Property(e => e.State);
            builder.Property(e => e.Result);
            builder.Property(e => e.Config);
        }

        #endregion
    }
}

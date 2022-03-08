using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Acolyte.Assertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectV.DataAccessLayer.Services.Jobs
{
    [Table("jobs")]
    public sealed class JobDbInfo
    {
        [Key, Required]
        [Column("id")]
        public Guid Id { get; }

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
            Id = id;
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

        #region Implementation of IEntityTypeConfiguration<JobDbInfo>

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

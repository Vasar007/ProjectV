using System;
using Acolyte.Assertions;

namespace ProjectV.Models.Internal.Jobs
{
    // Implicitly implements "IEquatable<JobId>" because it is record.
    public readonly record struct JobId
    {
        public static JobId None { get; } = new JobId(Guid.Empty);

        public Guid Value { get; }

        public bool IsSpecified => this != None;


        private JobId(
            Guid value)
        {
            Value = value;
        }

        public static JobId Create()
        {
            return new JobId(Guid.NewGuid());
        }

        public static JobId Wrap(Guid id)
        {
            id.ThrowIfEmpty(nameof(id));

            return new JobId(id);
        }
    }
}

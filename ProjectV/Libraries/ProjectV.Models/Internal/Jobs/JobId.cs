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

        public static JobId Parse(string rawId)
        {
            rawId.ThrowIfNullOrEmpty(nameof(rawId));

            var id = Guid.Parse(rawId);
            return Wrap(id);
        }

        public static bool TryParse(string? rawId, out JobId result)
        {
            if (Guid.TryParse(rawId, out Guid id))
            {
                result = Wrap(id);
                return true;
            }

            result = default;
            return false;
        }
    }
}

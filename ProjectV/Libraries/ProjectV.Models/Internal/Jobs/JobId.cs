using System;
using System.Globalization;
using Acolyte.Assertions;

namespace ProjectV.Models.Internal.Jobs
{
    public readonly struct JobId : IEquatable<JobId>
    {
        public static JobId None { get; } = new JobId(Guid.Empty);

        private readonly Guid _value;

        public Guid Value
        {
            get
            {
#if DEBUG
                _value.ThrowIfEmpty(nameof(_value));
#endif
                return _value;
            }
        }

        private JobId(Guid value)
        {
            _value = value;
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

        #region Object Overridden Methods

        public override string ToString()
        {
            return Value.ToString("N", CultureInfo.InvariantCulture);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not null and JobId taskId)
                return Equals(taskId);

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        #endregion

        #region IEquatable<JobId> Implementation

        public bool Equals(JobId other)
        {
            return Value.Equals(other.Value);
        }

        public static bool operator ==(JobId left, JobId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(JobId left, JobId right)
        {
            return !(left == right);
        }

        #endregion
    }
}

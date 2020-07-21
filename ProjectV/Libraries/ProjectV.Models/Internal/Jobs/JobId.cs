using System;
using System.Globalization;

namespace ProjectV.Models.Internal.Jobs
{
    public struct JobId : IEquatable<JobId>
    {
        public static JobId None { get; } = new JobId(Guid.Empty);

        public Guid Value { get; }

        private JobId(Guid value)
        {
            Value = value;
        }

        public static JobId Create()
        {
            return new JobId(Guid.NewGuid());
        }

        public static JobId Wrap(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(nameof(id), "Unique identifier is invalid.");

            return new JobId(id);
        }

        #region Object Overridden Methods

        public override string ToString()
        {
            return Value.ToString("N", CultureInfo.InvariantCulture);
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is JobId taskId)
                return Equals(taskId);

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        #endregion

        #region IEquatable<Rating> Implementation

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

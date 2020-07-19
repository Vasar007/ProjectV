using System;
using System.Globalization;

namespace ProjectV.Models.Internal.Tasks
{
    public struct TaskId : IEquatable<TaskId>
    {
        public static TaskId None { get; } = new TaskId(Guid.Empty);

        public Guid Value { get; }

        private TaskId(Guid value)
        {
            Value = value;
        }

        public static TaskId Create()
        {
            return new TaskId(Guid.NewGuid());
        }

        public static TaskId Wrap(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(nameof(id), "Unique identifier is invalid.");

            return new TaskId(id);
        }

        #region Object Overridden Methods

        public override string ToString()
        {
            return Value.ToString("N", CultureInfo.InvariantCulture);
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is TaskId taskId)
                return Equals(taskId);

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        #endregion

        #region IEquatable<Rating> Implementation

        public bool Equals(TaskId other)
        {
            return Value.Equals(other.Value);
        }

        public static bool operator ==(TaskId left, TaskId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TaskId left, TaskId right)
        {
            return !(left == right);
        }

        #endregion
    }
}

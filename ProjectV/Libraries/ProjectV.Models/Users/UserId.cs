using System;
using Acolyte.Assertions;

namespace ProjectV.Models.Users
{
    // Implicitly implements "IEquatable<UserId>" because it is record.
    public readonly record struct UserId
    {
        public static UserId None { get; } = new UserId(Guid.Empty);

        public Guid Value { get; }

        public bool IsSpecified => this != None;


        private UserId(
            Guid value)
        {
            Value = value;
        }

        public static UserId Create()
        {
            return new UserId(Guid.NewGuid());
        }

        public static UserId Wrap(Guid id)
        {
            id.ThrowIfEmpty(nameof(id));

            return new UserId(id);
        }
    }
}

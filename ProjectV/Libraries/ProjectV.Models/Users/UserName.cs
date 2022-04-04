using System;
using Acolyte.Assertions;

namespace ProjectV.Models.Users
{
    // Implicitly implements "IEquatable<UserName>" because it is record.
    public record struct UserName
    {
        public static UserName None { get; } = new UserName(string.Empty);

        public string Value { get; }

        public bool IsSpecified => this != None;


        private UserName(
            string value)
        {
            Value = value;
        }

        public static UserName Wrap(string userName)
        {
            userName.ThrowIfNullOrWhiteSpace(nameof(userName));

            return new UserName(userName);
        }

        public bool Equals(UserName other)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }
    }
}

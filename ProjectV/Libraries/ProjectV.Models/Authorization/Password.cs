using System;
using Acolyte.Assertions;

namespace ProjectV.Models.Authorization
{
    // Implicitly implements "IEquatable<Password>" because it is record.
    public readonly record struct Password
    {
        public static Password None { get; } = new Password(string.Empty);

        public string Value { get; }

        public bool IsSpecified => this != None;


        private Password(
            string value)
        {
            Value = value;
        }

        public static Password Wrap(string password)
        {
            password.ThrowIfNullOrWhiteSpace(nameof(password));

            return new Password(password);
        }

        public bool Equals(Password other)
        {
            return StringComparer.Ordinal.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }
    }
}

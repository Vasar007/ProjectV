using System;
using Acolyte.Assertions;

namespace ProjectV.Models.Authorization.Tokens
{
    // Implicitly implements "IEquatable<RefreshTokenId>" because it is record.
    public readonly record struct RefreshTokenId
    {
        public static RefreshTokenId None { get; } = new RefreshTokenId(Guid.Empty);

        public Guid Value { get; }

        public bool IsSpecified => this != None;


        private RefreshTokenId(
            Guid value)
        {
            Value = value;
        }

        public static RefreshTokenId Create()
        {
            return new RefreshTokenId(Guid.NewGuid());
        }

        public static RefreshTokenId Wrap(Guid id)
        {
            id.ThrowIfEmpty(nameof(id));

            return new RefreshTokenId(id);
        }

        public static RefreshTokenId Parse(string rawId)
        {
            rawId.ThrowIfNullOrEmpty(nameof(rawId));

            var id = Guid.Parse(rawId);
            return Wrap(id);
        }

        public static bool TryParse(string? rawId, out RefreshTokenId result)
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

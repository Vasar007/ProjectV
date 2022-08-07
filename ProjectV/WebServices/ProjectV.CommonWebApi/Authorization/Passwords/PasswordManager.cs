using System;
using System.Security.Cryptography;
using Acolyte.Assertions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using ProjectV.Models.Authorization;

namespace ProjectV.CommonWebApi.Authorization.Passwords
{
    public sealed class PasswordManager : IPasswordManager
    {
        public PasswordManager()
        {
        }

        #region IPasswordManager Implementation

        public byte[] GetSecureSalt()
        {
            // Starting .NET 6, the Class RNGCryptoServiceProvider is obsolete,
            // so now we have to use the RandomNumberGenerator Class to generate a secure random
            // number bytes.

            return RandomNumberGenerator.GetBytes(32);
        }

        public Password HashUsingPbkdf2(Password password, byte[] salt)
        {
            salt.ThrowIfNullOrEmpty(nameof(salt));

            byte[] derivedKey = KeyDerivation.Pbkdf2(
                password: password.Value,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32
            );

            var base64String = Convert.ToBase64String(derivedKey);
            return Password.Wrap(base64String);
        }

        public bool EnsurePasswordIsStrong(Password password)
        {
            // TODO: add normal password checker.
            return password.Value.Length > 7;
        }

        #endregion
    }
}

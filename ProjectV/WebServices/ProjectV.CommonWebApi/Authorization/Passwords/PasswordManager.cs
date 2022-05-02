using System;
using System.Security.Cryptography;
using Acolyte.Assertions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

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

        public string HashUsingPbkdf2(string password, byte[] salt)
        {
            password.ThrowIfNullOrWhiteSpace(nameof(password));
            salt.ThrowIfNullOrEmpty(nameof(salt));

            byte[] derivedKey = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32
            );

            return Convert.ToBase64String(derivedKey);
        }

        public bool EnsurePasswordIsStrong(string password)
        {
            password.ThrowIfNullOrWhiteSpace(nameof(password));

            // TODO: add normal password checker.
            return password.Length <= 7;
        }

        #endregion
    }
}

using ProjectV.Models.Authorization;

namespace ProjectV.CommonWebApi.Authorization.Passwords
{
    public interface IPasswordManager
    {
        byte[] GetSecureSalt();

        string HashUsingPbkdf2(Password password, byte[] salt);

        bool EnsurePasswordIsStrong(Password password);
    }
}

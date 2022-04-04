namespace ProjectV.CommonWebApi.Authorization.Passwords
{
    public interface IPasswordManager
    {
        byte[] GetSecureSalt();

        string HashUsingPbkdf2(string password, byte[] salt);

        bool EnsurePasswordIsStrong(string password);
    }
}

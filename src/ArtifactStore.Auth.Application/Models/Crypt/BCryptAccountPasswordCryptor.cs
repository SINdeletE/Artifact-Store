using ArtifactStore.Auth.Application.Interfaces.Crypt;

namespace ArtifactStore.Auth.Application.Models.Crypt;

public class BCryptAccountPasswordCryptor : IAccountPasswordCryptor
{
    public string Crypt(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    public bool Compare(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
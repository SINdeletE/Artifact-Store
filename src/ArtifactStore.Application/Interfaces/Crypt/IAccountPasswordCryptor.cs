namespace ArtifactStore.Application.Interfaces.Crypt;

public interface IAccountPasswordCryptor
{
    public string Crypt(string password);
    public bool Compare(string password, string hashedPassword);
}
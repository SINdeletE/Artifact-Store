namespace ArtifactStore.Auth.Application.Models.Requires.Account;

public class AddAccountRequire
{
    public string Role { get; set; }
    public string Nickname { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}
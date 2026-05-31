namespace ArtifactStore.Auth.Application.Models.Requires.Account;

public class UpdateAccountRequire
{
    public Guid Id { get; set; }
    public string Nickname { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public DateOnly RegisterDate { get; set; }  
}
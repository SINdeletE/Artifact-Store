namespace ArtifactStore.Auth.Application.Models.Requires.Token;

public class GenerateTokenRequire
{
    public ArtifactStore.Auth.Domain.Entities.Account Account { get; set; }

    public GenerateTokenRequire(){}
    public GenerateTokenRequire(ArtifactStore.Auth.Domain.Entities.Account account)
    {
        Account = account;
    }
}
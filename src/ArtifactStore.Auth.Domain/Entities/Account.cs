using ArtifactStore.Auth.Domain.Common;

namespace ArtifactStore.Auth.Domain.Entities;

public class Account : IEntity
{
    public Guid Id { get; set; }
    
    public Guid AccountRoleId { get; set; }

    public AccountRole AccountRole
    {
        get;
        set
        {
            field = value;
            AccountRoleId = value.Id;
        }
    }

    public string Nickname { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public DateOnly RegisterDate { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    
    public uint Version { get; set; }
    
    public Account(){}

    public Account(Guid id, AccountRole accountRole, string nickname, string password, string email, DateOnly registerDate, DateTimeOffset? deletedAt)
    {
        Id = id;
        AccountRole = accountRole;
        Nickname = nickname;
        Password = password;
        Email = email;
        RegisterDate = registerDate;
        DeletedAt = deletedAt;
    }
}
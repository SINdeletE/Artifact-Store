namespace ArtifactStore.Application.Models.Requires.Inventory;

public class DeleteInventoryItemRequire
{
    public Guid Id { get; set; }
    
    public Guid AccountId { get; set; }
    public bool IsPrivileged { get; set; }
}
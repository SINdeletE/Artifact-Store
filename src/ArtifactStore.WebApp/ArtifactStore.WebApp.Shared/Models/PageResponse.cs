namespace ArtifactStore.WebApp.Shared.Models;

public sealed class PageResponse<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalItems { get; set; }
}

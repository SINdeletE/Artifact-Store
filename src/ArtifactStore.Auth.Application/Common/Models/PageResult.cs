namespace ArtifactStore.Auth.Application.Common.Models;

public class PageResult<T>
{
    public IEnumerable<T> Items  { get; set; } = Enumerable.Empty<T>();
    public int TotalItems { get; set; }

    public PageResult(IEnumerable<T> items, int totalItems)
    {
        Items = items;
        TotalItems = totalItems;
    }
}
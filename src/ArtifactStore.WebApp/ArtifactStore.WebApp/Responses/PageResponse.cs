namespace ArtifactStore.WebApp.Responses;

public class PageResponse<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalItems { get; set; }
}

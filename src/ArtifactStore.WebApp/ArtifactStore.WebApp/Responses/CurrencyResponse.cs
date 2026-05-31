namespace ArtifactStore.WebApp.Responses;

public class CurrencyResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public DateTimeOffset? DeletedAt { get; set; }
}

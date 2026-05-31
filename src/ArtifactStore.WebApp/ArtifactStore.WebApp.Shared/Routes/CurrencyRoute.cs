namespace ArtifactStore.WebApp.Shared.Routes;

public class CurrencyRoute
{
    public const string BaseUrl = "api/currencies";
    public const string GetById = $"{BaseUrl}/{{id:guid}}";

    public static string GetByIdUrl(Guid id) => $"{BaseUrl}/{id}";
}

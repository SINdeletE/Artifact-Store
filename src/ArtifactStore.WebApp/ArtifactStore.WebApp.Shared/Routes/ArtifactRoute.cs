namespace ArtifactStore.WebApp.Shared.Routes;

public class ArtifactRoute
{
    public const string BaseUrl = "api/artifact";
    public const string Add = BaseUrl;
    public const string Update = BaseUrl;
    public const string GetById = $"{BaseUrl}/{{id:guid}}";

    public static string GetByIdUrl(Guid id) => $"{BaseUrl}/{id}";
}

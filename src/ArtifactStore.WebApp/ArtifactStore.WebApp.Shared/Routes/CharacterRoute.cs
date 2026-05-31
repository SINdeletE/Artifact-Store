namespace ArtifactStore.WebApp.Shared.Routes;

public class CharacterRoute
{
    public const string BaseUrl = "api/character";
    public const string InternalUrl = "internal/character";
    
    public const string Add = BaseUrl;
    public const string GetPage = $"{BaseUrl}/page";
    public const string GetUserPage = $"{BaseUrl}/my/page";
    public const string GetUserAll = $"{BaseUrl}/my/all";
    public const string GetById = $"{BaseUrl}/{{id:guid}}";
    public const string Delete = $"{BaseUrl}/{{id:guid}}";
    
    public const string Choose = $"{InternalUrl}/choose";

    public static string GetByIdUrl(Guid id) => $"{BaseUrl}/{id}";
    public static string DeleteUrl(Guid id) => $"{BaseUrl}/{id}";
}

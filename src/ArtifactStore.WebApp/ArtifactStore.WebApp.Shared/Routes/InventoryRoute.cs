namespace ArtifactStore.WebApp.Shared.Routes;

public class InventoryRoute
{
    public const string BaseUrl = "api/inventory";
    public const string Add = BaseUrl;
    public const string GetById = $"{BaseUrl}/{{id:guid}}";
    public const string Delete = $"{BaseUrl}/{{id:guid}}";
    public const string GetByCharacterId = $"{BaseUrl}/character/{{characterId:guid}}";
    public const string GetPage = $"{BaseUrl}/my";

    public static string GetByIdUrl(Guid id) => $"{BaseUrl}/{id}";
    public static string DeleteUrl(Guid id) => $"{BaseUrl}/{id}";
    public static string GetByCharacterIdUrl(Guid characterId) => $"{BaseUrl}/character/{characterId}";
    public static string GetPageUrl(int page, int pageSize) =>
        $"{GetPage}?page={page}&pageSize={pageSize}";
    public static string GetPageUrl(Guid characterId, int page, int pageSize) =>
        $"{GetPage}?characterId={characterId}&page={page}&pageSize={pageSize}";
}

namespace ArtifactStore.WebApp.Shared.Routes;

public class StoreRoute
{
    public const string BaseUrl = "api/store";
    public const string Add = BaseUrl;
    public const string GetPage = $"{BaseUrl}/page";
    public const string Delete = $"{BaseUrl}/{{id:guid}}";
    public const string Update = $"{BaseUrl}/{{id:guid}}";
    public const string Purchase = $"{BaseUrl}/purchase";
    public const string Sell = $"{BaseUrl}/sell";

    public static string DeleteUrl(Guid id) => $"{BaseUrl}/{id}";
    public static string UpdateUrl(Guid id) => $"{BaseUrl}/{id}";
    public static string GetPageUrl(int page, int pageSize) => $"{GetPage}?page={page}&pageSize={pageSize}";
    public static string PurchaseUrl(Guid storeItemId, Guid characterId) =>
        $"{Purchase}?storeItemId={storeItemId}&characterId={characterId}";
}

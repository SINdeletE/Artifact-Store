namespace ArtifactStore.WebApp.Shared.Routes;

public class BalanceRoute
{
    public const string BaseUrl = "api/balance";
    public const string AddWithCurrency = BaseUrl;
    public const string GetById = $"{BaseUrl}/{{id:guid}}";
    public const string GetCharacterBalances = $"{BaseUrl}/character";
    public const string Update = $"{BaseUrl}/{{id:guid}}";

    public static string GetByIdUrl(Guid id) => $"{BaseUrl}/{id}";
    public static string GetCharacterBalancesUrl(Guid characterId) => $"{GetCharacterBalances}?id={characterId}";
    public static string UpdateUrl(Guid id) => $"{BaseUrl}/{id}";
}

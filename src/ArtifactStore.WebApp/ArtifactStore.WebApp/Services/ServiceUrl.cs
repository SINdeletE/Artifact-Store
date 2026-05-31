namespace ArtifactStore.WebApp.Services;

internal static class ServiceUrl
{
    public static string WithPage(string url, int page, int pageSize) =>
        $"{url}?page={page}&pageSize={pageSize}";
}

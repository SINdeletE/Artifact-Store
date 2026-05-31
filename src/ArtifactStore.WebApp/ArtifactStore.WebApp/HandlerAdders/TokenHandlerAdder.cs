using System.Net.Http.Headers;

namespace ArtifactStore.WebApp.HandlerAdders;

public static class TokenHandlerAdder
{
    public static void Add(HttpClient client, IHttpContextAccessor accessor)
    {
        var token = accessor.HttpContext?.User.FindFirst("access_token")?.Value;
        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
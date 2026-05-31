using System.Text.Json;
using ArtifactStore.WebApp.Exceptions;
using ArtifactStore.WebApp.Responses;
using ArtifactStore.WebApp.Shared.Responses;

namespace ArtifactStore.WebApp.Error;

public class ErrorHandler : IErrorHandler
{
    public async Task EnsureSuccessOrThrow(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        if (response.Content.Headers.ContentLength == 0)
        {
            throw new ApiException(response.ReasonPhrase ?? "Invalid error response", (int)response.StatusCode);
        }
        
        ErrorResponse? error = null;
        try
        {
            error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        }
        catch (JsonException)
        {
            throw new ApiException(response.ReasonPhrase ?? "Invalid error response", (int)response.StatusCode);
        }
        
        if (error is not null && error.Error is not null)
            throw new ApiException(error.Error, (int)response.StatusCode);

        throw new ApiException("Invalid error response", (int)response.StatusCode);
    }
}

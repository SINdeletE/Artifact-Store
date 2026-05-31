namespace ArtifactStore.WebApp.Error;

public interface IErrorHandler
{
    public Task EnsureSuccessOrThrow(HttpResponseMessage response);
}
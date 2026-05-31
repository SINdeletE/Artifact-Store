namespace ArtifactStore.Auth.Domain.Exceptions;

public class RetryConnectionException : ServerException
{
    public RetryConnectionException(string message) : base(message) {}
}
namespace ArtifactStore.Auth.Domain.Exceptions;

public class ForbiddenException : ServerException
{
    public ForbiddenException(string message) : base(message)
    {}
}
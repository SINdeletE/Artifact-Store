namespace ArtifactStore.Domain.Exceptions;

public class UnauthorizedException : ServerException
{
    public UnauthorizedException(string message) : base(message)
    {}
}
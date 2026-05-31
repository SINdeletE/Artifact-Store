namespace ArtifactStore.Auth.Domain.Exceptions;

public class InternalServerErrorException : ServerException
{
    public InternalServerErrorException(string message) : base(message)
    {}
}
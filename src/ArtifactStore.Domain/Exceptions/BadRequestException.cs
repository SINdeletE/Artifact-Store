namespace ArtifactStore.Domain.Exceptions;

public class BadRequestException : ServerException
{
    public BadRequestException(string message) : base(message)
    {}
}
namespace ArtifactStore.Auth.Domain.Exceptions;

public class ConflictException : ServerException
{
    public ConflictException(string message) : base(message) {}
}
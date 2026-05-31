namespace ArtifactStore.Domain.Exceptions;

public class NotFoundException : ServerException
{
    public NotFoundException(string message) : base(message)
    {}
}
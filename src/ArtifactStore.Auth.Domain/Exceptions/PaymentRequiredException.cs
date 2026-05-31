namespace ArtifactStore.Auth.Domain.Exceptions;

public class PaymentRequiredException : ServerException
{
    public PaymentRequiredException(string message) : base(message)
    {}
}
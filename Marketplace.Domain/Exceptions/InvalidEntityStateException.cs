namespace Marketplace.Domain.Exceptions;

public class InvalidEntityStateException : Exception
{
    public InvalidEntityStateException(object entity, string message) : 
        base($"Events {entity.GetType().Name} state change rejected {message} ")
    {
    }
}
namespace Parcorpus.Core.Exceptions;

public class InvalidPagingException : Exception
{
    public InvalidPagingException()
    {
    }

    public InvalidPagingException(string message) : base(message)
    {
    }

    public InvalidPagingException(string message, Exception inner) : base(message, inner)
    {
    }
}
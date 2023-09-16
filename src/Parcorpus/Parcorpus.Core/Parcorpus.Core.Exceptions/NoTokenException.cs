namespace Parcorpus.Core.Exceptions;

public class NoTokenException : Exception
{
    public NoTokenException()
    {
    }

    public NoTokenException(string message) : base(message)
    {
    }

    public NoTokenException(string message, Exception inner) : base(message, inner)
    {
    }
}
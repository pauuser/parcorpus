namespace Parcorpus.Core.Exceptions;

public class TextAlreadyExistsException : Exception
{
    public TextAlreadyExistsException()
    {
    }

    public TextAlreadyExistsException(string message) : base(message)
    {
    }

    public TextAlreadyExistsException(string message, Exception inner) : base(message, inner)
    {
    }
}
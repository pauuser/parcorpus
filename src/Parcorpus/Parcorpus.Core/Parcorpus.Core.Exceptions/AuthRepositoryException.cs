namespace Parcorpus.Core.Exceptions;

public class AuthRepositoryException : Exception
{
    public AuthRepositoryException()
    {
    }

    public AuthRepositoryException(string message) : base(message)
    {
    }

    public AuthRepositoryException(string message, Exception inner) : base(message, inner)
    {
    }
}
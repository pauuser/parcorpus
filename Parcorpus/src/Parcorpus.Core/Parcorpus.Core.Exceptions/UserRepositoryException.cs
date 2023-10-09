namespace Parcorpus.Core.Exceptions;

public class UserRepositoryException : Exception
{
    public UserRepositoryException()
    {
    }

    public UserRepositoryException(string message) : base(message)
    {
    }

    public UserRepositoryException(string message, Exception inner) : base(message, inner)
    {
    }
}
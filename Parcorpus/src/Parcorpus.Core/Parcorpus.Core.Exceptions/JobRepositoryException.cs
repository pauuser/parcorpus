namespace Parcorpus.Core.Exceptions;

public class JobRepositoryException : Exception
{
    public JobRepositoryException()
    {
    }

    public JobRepositoryException(string message) : base(message)
    {
    }

    public JobRepositoryException(string message, Exception inner) : base(message, inner)
    {
    }
}
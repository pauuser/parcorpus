namespace Parcorpus.Core.Exceptions;

public class CredentialsRepositoryException : Exception
{
    public CredentialsRepositoryException()
    {
    }

    public CredentialsRepositoryException(string message) : base(message)
    {
    }

    public CredentialsRepositoryException(string message, Exception inner) : base(message, inner)
    {
    }
}
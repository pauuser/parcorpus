namespace Parcorpus.Core.Exceptions;

public class LanguageRepositoryException : Exception
{
    public LanguageRepositoryException()
    {
    }

    public LanguageRepositoryException(string message) : base(message)
    {
    }

    public LanguageRepositoryException(string message, Exception inner) : base(message, inner)
    {
    }
}
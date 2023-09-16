namespace Parcorpus.Core.Exceptions;

public class SearchHistoryRepositoryException : Exception
{
    public SearchHistoryRepositoryException()
    {
    }

    public SearchHistoryRepositoryException(string message) : base(message)
    {
    }

    public SearchHistoryRepositoryException(string message, Exception inner) : base(message, inner)
    {
    }
}
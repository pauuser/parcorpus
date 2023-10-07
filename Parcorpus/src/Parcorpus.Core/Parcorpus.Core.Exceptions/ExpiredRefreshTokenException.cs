namespace Parcorpus.Core.Exceptions;

public class ExpiredRefreshTokenException : Exception
{
    public ExpiredRefreshTokenException()
    {
    }

    public ExpiredRefreshTokenException(string message) : base(message)
    {
    }

    public ExpiredRefreshTokenException(string message, Exception inner) : base(message, inner)
    {
    }   
}
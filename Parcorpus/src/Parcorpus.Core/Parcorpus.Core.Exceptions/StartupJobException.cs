namespace Parcorpus.Core.Exceptions;

public class StartupJobException : Exception
{
    public StartupJobException()
    {
    }

    public StartupJobException(string message) : base(message)
    {
    }

    public StartupJobException(string message, Exception inner) : base(message, inner)
    {
    }
}
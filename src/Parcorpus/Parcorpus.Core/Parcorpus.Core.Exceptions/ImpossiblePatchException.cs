namespace Parcorpus.Core.Exceptions;

public class ImpossiblePatchException : Exception
{
    public ImpossiblePatchException()
    {
    }

    public ImpossiblePatchException(string message) : base(message)
    {
    }

    public ImpossiblePatchException(string message, Exception inner) : base(message, inner)
    {
    }   
}
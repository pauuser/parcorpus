namespace Parcorpus.Core.Exceptions;

public class QueueProducerException : Exception
{
    public QueueProducerException()
    {
    }

    public QueueProducerException(string message) : base(message)
    {
    }

    public QueueProducerException(string message, Exception inner) : base(message, inner)
    {
    }
}
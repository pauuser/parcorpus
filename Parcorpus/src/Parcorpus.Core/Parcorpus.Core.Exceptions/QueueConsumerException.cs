namespace Parcorpus.Core.Exceptions;

public class QueueConsumerException : Exception
{
    public QueueConsumerException()
    {
    }

    public QueueConsumerException(string message) : base(message)
    {
    }

    public QueueConsumerException(string message, Exception inner) : base(message, inner)
    {
    }
}
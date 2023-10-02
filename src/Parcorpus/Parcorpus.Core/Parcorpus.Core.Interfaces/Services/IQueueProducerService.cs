namespace Parcorpus.Core.Interfaces;

public interface IQueueProducerService
{
    Task SendMessage<T>(T message);
}
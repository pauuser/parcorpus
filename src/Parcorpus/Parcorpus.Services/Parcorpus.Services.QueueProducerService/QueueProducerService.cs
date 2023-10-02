using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Parcorpus.Core.Configuration;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using RabbitMQ.Client;

namespace Parcorpus.Services.QueueProducerService;

public class QueueProducerService : IQueueProducerService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    private readonly QueueConfiguration _configuration;
    private readonly ILogger<QueueProducerService> _logger;
    
    public QueueProducerService(IOptions<QueueConfiguration> configuration, ILogger<QueueProducerService> logger)
    {
        _configuration = configuration.Value ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var connectionFactory = new ConnectionFactory()
        {
            Uri = new Uri(_configuration.ConnectionString)
        };

        _connection = connectionFactory.CreateConnection();
        _logger.LogInformation("Producer opened connection to queue");
        
        _channel = _connection.CreateModel();
        _logger.LogInformation("Producer created model for queue");

        _channel.QueueDeclare(queue: _configuration.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        _logger.LogInformation("Producer declared queue {name}", _configuration.QueueName);
    } 
    
    public Task SendMessage<T>(T message)
    {
        try
        {
            var serialized = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(serialized);

            _logger.LogInformation("Sending message {message} to queue", message);
            lock (_channel)
            {
                _channel.BasicPublish(exchange: string.Empty,
                    routingKey: _configuration.QueueName,
                    basicProperties: null,
                    body: body);
            }
            _logger.LogInformation("Message {message} successfully sent", message);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to send message {message} to queue: {ex}", message, ex.Message);
            throw new QueueProducerException($"Failed to send message to queue: {ex.Message}");
        }
    }
}
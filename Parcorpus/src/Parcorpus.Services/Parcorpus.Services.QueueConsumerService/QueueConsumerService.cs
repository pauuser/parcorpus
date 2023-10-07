using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Parcorpus.Core.Configuration;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;
using Parcorpus.Core.Models.Enums;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Parcorpus.Services.QueueConsumerService;

public class QueueConsumerService : BackgroundService, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    private readonly QueueConfiguration _configuration;
    private readonly ILogger<QueueConsumerService> _logger;

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IAnnotationService _annotationService;

    public QueueConsumerService(IOptions<QueueConfiguration> configuration, 
        ILogger<QueueConsumerService> logger,
        IServiceScopeFactory serviceScopeFactory,
        IAnnotationService annotationService)
    {
        _configuration = configuration.Value ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _annotationService = annotationService ?? throw new ArgumentNullException(nameof(annotationService));
        
        var connectionFactory = new ConnectionFactory()
        {
            Uri = new Uri(_configuration.ConnectionString),
            DispatchConsumersAsync = true,
            ConsumerDispatchConcurrency = 3
        };

        _connection = connectionFactory.CreateConnection();
        _logger.LogInformation("Consumer opened connection to queue");
        
        _channel = _connection.CreateModel();
        _logger.LogInformation("Consumer created model for queue");

        _channel.QueueDeclare(queue: _configuration.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        _logger.LogInformation("Consumer declared queue {name}", _configuration.QueueName);
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += UploadText;
        _logger.LogInformation("Callback for consumer was set");
        
        _channel.BasicConsume(queue: _configuration.QueueName, autoAck: true, consumer: consumer);
        
        return Task.CompletedTask;
    }

    private async Task UploadText(object ch, BasicDeliverEventArgs args)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var jobRepository = scope.ServiceProvider.GetService<IJobRepository>();
        
        var bodyString = Encoding.UTF8.GetString(args.Body.ToArray());
        var job = JsonSerializer.Deserialize<UploadJob>(bodyString);
        _logger.LogInformation("New upload job was deserialized by consumer, job id {id}", job.JobId);

        try
        {
            var text = job!.BiText;
            var userId = job.UserId;

            if (string.IsNullOrWhiteSpace(text.SourceText) || string.IsNullOrWhiteSpace(text.TargetText))
            {
                _logger.LogError("One of the texts is null or whitespace. UserId: {userId}", userId);
                throw new ArgumentException($"One of the texts is null or whitespace. UserId: {userId}");
            }

            var languageRepository = scope.ServiceProvider.GetService<ILanguageRepository>();

            var textExists = await languageRepository!.TextExists(metaAnnotation: text.MetaAnnotation,
                sourceLanguage: text.SourceLanguage,
                targetLanguage: text.TargetLanguage);
            if (textExists)
            {
                _logger.LogError("Text {author} \"{title}\" already exists and will not be added",
                    text.MetaAnnotation.Author, text.MetaAnnotation.Title);
                throw new TextAlreadyExistsException(
                    $"Text {text.MetaAnnotation.Author} \"{text.MetaAnnotation.Title}\" " +
                    $"already exists and will not be added");
            }
            
            await jobRepository.UpdateStatus(job.JobId, JobStatus.Aligning);
            
            var alignedSentences = await _annotationService.AlignSentencesWithWords(text);
            
            await jobRepository.UpdateStatus(job.JobId, JobStatus.Saving);
            
            await languageRepository.AddAlignedText(userId, new Text(textId: default,
                title: text.MetaAnnotation.Title,
                author: text.MetaAnnotation.Author,
                source: text.MetaAnnotation.Source,
                creationYear: text.MetaAnnotation.CreationYear,
                addDate: text.MetaAnnotation.AddDate,
                sourceLanguage: text.SourceLanguage,
                targetLanguage: text.TargetLanguage,
                genres: text.Genres,
                sentences: alignedSentences,
                addedBy: userId));
            
            await jobRepository.UpdateStatus(job.JobId, JobStatus.Finished);
        }
        catch (TextAlreadyExistsException)
        {
            await jobRepository.UpdateStatus(job.JobId, JobStatus.Failed);
            throw;
        }
        catch (ArgumentException)
        {
            await jobRepository.UpdateStatus(job.JobId, JobStatus.Failed);
            throw;
        }
        catch (Exception ex)
        {
            await jobRepository.UpdateStatus(job.JobId, JobStatus.Failed);
            _logger.LogError("Error during aligning text: {message}", ex.Message);
            throw new QueueConsumerException($"Error during aligning text: {ex.Message}");
        }
    }
}
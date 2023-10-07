using Aligner;
using Castle.Core.Logging;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Parcorpus.Core.Configuration;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;

namespace Parcorpus.Services.AnnotationService.WordAlignerClient;

public sealed class WordAligner : IWordAligner, IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly Aligner.WordAligner.WordAlignerClient _client;
    private readonly ILogger<WordAligner> _logger;

    private bool _disposed = false;
    
    public WordAligner(IOptions<WordAlignerConfiguration> configuration, ILogger<WordAligner> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        var alignerConfiguration = configuration.Value ?? throw new ArgumentNullException(nameof(configuration));
        
        _logger.LogInformation("Connecting to {server}", alignerConfiguration.WordAlignerServer);
        _channel = GrpcChannel.ForAddress(alignerConfiguration.WordAlignerServer);
        _client = new Aligner.WordAligner.WordAlignerClient(_channel);
    }
        
    public async Task<List<WordCorrespondence>> AlignWords(string sourceText, string targetText, Language sourceLanguage, Language targetLanguage)
    {
        var reply = await _client.AlignWordsAsync(new SentencesRequest
            {
                SourceText =  sourceText,
                TargetText = targetText,
                SourceLanguageFullName = sourceLanguage.FullEnglishName,
                TargetLanguageFullName = targetLanguage.FullEnglishName
            }
        );

        var result = reply.Words.Select(elem => 
            new WordCorrespondence
            {
                SourceWord = new Word(elem.SourceWord, sourceLanguage),
                AlignedWord = new Word(elem.TargetWord, targetLanguage)
            }).ToList();

        return result;
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _channel.Dispose();
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        _disposed = true;
        
        GC.SuppressFinalize(this);
    }

    ~WordAligner()
    {
        Dispose(false);
    }
}
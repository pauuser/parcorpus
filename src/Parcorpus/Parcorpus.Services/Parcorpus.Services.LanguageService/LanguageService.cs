using Microsoft.Extensions.Logging;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;
using Parcorpus.Core.Models.Enums;

namespace Parcorpus.Services.LanguageService;

public class LanguageService : ILanguageService
{
    private readonly ILanguageRepository _languageRepository;
    private readonly ISearchHistoryRepository _searchHistoryRepository;
    private readonly ILogger<LanguageService> _logger;

    public LanguageService(ILanguageRepository languageRepository, 
        IAnnotationService annotationService, 
        ISearchHistoryRepository searchHistoryRepository,
        IQueueProducerService queueProducerService,
        IJobRepository jobRepository,
        ILogger<LanguageService> logger)
    {
        _languageRepository = languageRepository ?? throw new ArgumentNullException(nameof(languageRepository));
        _searchHistoryRepository =
            searchHistoryRepository ?? throw new ArgumentNullException(nameof(searchHistoryRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<List<Concordance>> GetConcordance(Guid userId, ConcordanceQuery query)
    {
        var word = query.SourceWord;
        var desiredLanguage = query.DestinationLanguage;
        var filter = query.Filters;
        
        await _searchHistoryRepository.AddRecord(userId, query);
        
        var concordance = await _languageRepository.GetConcordance(word, desiredLanguage, filter);
        if (!concordance.Any())
        {
            _logger.LogError("Nothing was found for word \"{word}\" ({srcLang} -> {trgLang}) and filter {filter}", 
                word.WordForm, word.Language.ShortName, desiredLanguage.ShortName, filter.ToString());
            throw new NotFoundException($"Nothing was found for word \"{word.WordForm}\" ({word.Language.ShortName} -> {desiredLanguage.ShortName})");
        }

        return concordance;
    }

    public async Task DeleteText(Guid userId, int textId)
    {
        await _languageRepository.DeleteTextById(userId, textId);
    }
    
    public async Task<List<Text>> GetTextsAddedByUser(Guid userId)
    {
        var texts = await _languageRepository.GetTextsAddedByUser(userId);

        if (!texts.Any())
        {
            _logger.LogError("Texts for user {userId} don't exist", userId);
            throw new NotFoundException($"Texts for user {userId} don't exist");
        }

        return texts;
    }

    public async Task<Text> GetTextById(int textId)
    {
        var text = await _languageRepository.GetTextById(textId);
        
        return text;
    }
}
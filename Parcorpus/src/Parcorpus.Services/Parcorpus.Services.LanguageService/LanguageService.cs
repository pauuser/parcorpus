using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Parcorpus.Core.Configuration;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;

namespace Parcorpus.Services.LanguageService;

public class LanguageService : ILanguageService
{
    private readonly ILanguageRepository _languageRepository;
    private readonly ISearchHistoryRepository _searchHistoryRepository;
    
    private readonly ILogger<LanguageService> _logger;

    private readonly PagingConfiguration _pagingConfiguration;

    public LanguageService(ILanguageRepository languageRepository,
        ISearchHistoryRepository searchHistoryRepository,
        IOptions<PagingConfiguration> pagingConfiguration,
        ILogger<LanguageService> logger)
    {
        _languageRepository = languageRepository ?? throw new ArgumentNullException(nameof(languageRepository));
        _searchHistoryRepository =
            searchHistoryRepository ?? throw new ArgumentNullException(nameof(searchHistoryRepository));
        _pagingConfiguration =
            pagingConfiguration.Value ?? throw new ArgumentNullException(nameof(pagingConfiguration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<Paged<Concordance>> GetConcordance(Guid userId, ConcordanceQuery query, PaginationParameters paging)
    {
        if (paging.Specified && (paging.PageSize < _pagingConfiguration.MinPageSize || paging.PageSize > _pagingConfiguration.MaxPageSize))
        {
            _logger.LogError("Invalid paging to get concordance for user {userId}: {paging}", userId, paging);
            throw new InvalidPagingException($"Invalid paging to get concordance for user {userId}: {paging}");
        }
        
        var word = query.SourceWord;
        var desiredLanguage = query.DestinationLanguage;
        var filter = query.Filters;
        
        await _searchHistoryRepository.AddRecord(userId, query);
        
        var concordance = await _languageRepository.GetConcordance(word, desiredLanguage, filter, paging);
        if (!concordance.Items.Any())
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
    
    public async Task<Paged<Text>> GetTextsAddedByUser(Guid userId, PaginationParameters paging)
    {
        if (paging.Specified && (paging.PageSize < _pagingConfiguration.MinPageSize || paging.PageSize > _pagingConfiguration.MaxPageSize))
        {
            _logger.LogError("Invalid paging to get texts added by user {userId}: {paging}", userId, paging);
            throw new InvalidPagingException($"Invalid paging to get texts added by user {userId}: {paging}");
        }
        
        var texts = await _languageRepository.GetTextsAddedByUser(userId, paging);

        if (!texts.Items.Any())
        {
            _logger.LogError("Texts for user {userId} don't exist", userId);
            throw new NotFoundException($"Texts for user {userId} don't exist");
        }

        return texts;
    }

    public async Task<PagedText> GetTextById(int textId, PaginationParameters paging)
    {
        if (paging.Specified && (paging.PageSize < _pagingConfiguration.MinPageSize || paging.PageSize > _pagingConfiguration.MaxPageSize))
        {
            _logger.LogError("Invalid paging to get text {textId}: {paging}", textId, paging);
            throw new InvalidPagingException($"Invalid paging to get text {textId}: {paging}");
        }
        
        var text = await _languageRepository.GetTextById(textId, paging);
        
        return text;
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Context;
using Parcorpus.DataAccess.Converters;
using Parcorpus.DataAccess.Models;

namespace Parcorpus.DataAccess.Repositories;

public class SearchHistoryRepository : BaseRepository<SearchHistoryRepository>, ISearchHistoryRepository
{
    private readonly ParcorpusDbContext _context;

    public SearchHistoryRepository(ILogger<SearchHistoryRepository> logger, 
        ParcorpusDbContext context) : base(logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddRecord(Guid userId, ConcordanceQuery query)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user is null)
            {
                Logger.LogError("User with id = {userId} not found, cannot add search history", userId);
                throw new NotFoundException("User with id = {userId} not found, cannot add search history");
            }
            
            var searchHistory = new SearchHistoryDbModel(searchHistoryId: default,
                userId: userId,
                query: new HistoryJsonDbModel(word: query.SourceWord.WordForm,
                    sourceLanguageShortName: query.SourceWord.Language.ShortName,
                    destinationLanguageShortName: query.DestinationLanguage.ShortName,
                    filter: FilterConverter.ConvertAppModelToDbModel(query.Filters)),
                queryTimestampUtc: DateTime.UtcNow);
            searchHistory.UserNavigation = user;
            await _context.SearchHistory.AddAsync(searchHistory);

            await _context.SaveChangesAsync();
            Logger.LogInformation("Search history record for userId = {userId} was added", userId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during adding search history: {message}", ex.Message);
            throw new SearchHistoryRepositoryException($"Error during adding search history: {ex.Message}");
        }
    }

    public async Task<List<SearchHistoryRecord>> GetSearchHistory(Guid userId)
    {
        try
        {
            var history = await _context.SearchHistory.Where(sh => sh.UserId == userId).ToListAsync();

            return history.Select(SearchHistoryConverter.ConvertDbModelToAppModel).ToList();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during getting search history for userId = {userId}", userId);
            throw new SearchHistoryRepositoryException($"Error during getting search history for userId = {userId}");
        }
    }
}
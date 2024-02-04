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

            var existingHistory = (await _context.SearchHistory
                .Where(h => h.UserId == userId)
                .ToListAsync())
                .MaxBy(h => h.QueryTimestampUtc);
            if (existingHistory?.Query.Word == query.SourceWord.WordForm)
            {
                Logger.LogInformation("Word {word} is added to history and will not we recorded again for user {userId}", 
                    query.SourceWord.WordForm, userId);
            }
            else
            {
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
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during adding search history: {message}", ex.Message);
            throw new SearchHistoryRepositoryException($"Error during adding search history: {ex.Message}");
        }
    }

    public async Task<Paged<SearchHistoryRecord>> GetSearchHistory(Guid userId, PaginationParameters paging)
    {
        try
        {
            var history = _context.SearchHistory
                .Where(sh => sh.UserId == userId)
                .OrderByDescending(h => h.QueryTimestampUtc);
            var totalCount = await history.CountAsync();

            if (paging.Specified)
            {
                if (paging.OutOfRange(totalCount))
                {
                    Logger.LogError("Paging error: {paging} is invalid for totalCount = {totalCount}", paging,
                        totalCount);
                    throw new InvalidPagingException(
                        $"Paging error: {paging} is invalid for totalCount = {totalCount}");
                }

                history = history.Skip((paging.PageNumber!.Value - 1) * paging.PageSize!.Value)
                    .Take(paging.PageSize.Value)
                    .OrderByDescending(h => h.QueryTimestampUtc);
            }

            var result = await history.ToListAsync();

            return new Paged<SearchHistoryRecord>(pageNumber: paging.PageNumber,
                pageSize: paging.PageSize,
                totalCount: totalCount,
                items: result.Select(SearchHistoryConverter.ConvertDbModelToAppModel).ToList());
        }
        catch (InvalidPagingException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during getting search history for userId = {userId}: {message}", userId, ex.Message);
            throw new SearchHistoryRepositoryException($"Error during getting search history for userId = {userId}");
        }
    }
}
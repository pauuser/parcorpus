using Parcorpus.Core.Models;

namespace Parcorpus.Core.Interfaces;

public interface ISearchHistoryRepository
{
    Task AddRecord(Guid userId, ConcordanceQuery query);

    Task<List<SearchHistoryRecord>> GetSearchHistory(Guid userId);
}
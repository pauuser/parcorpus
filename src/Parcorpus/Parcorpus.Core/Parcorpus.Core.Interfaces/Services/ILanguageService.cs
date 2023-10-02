using Parcorpus.Core.Models;

namespace Parcorpus.Core.Interfaces;

public interface ILanguageService
{
    Task<Paged<Concordance>> GetConcordance(Guid userId, ConcordanceQuery query, PaginationParameters paging);

    Task DeleteText(Guid userId, int textId);

    Task<Paged<Text>> GetTextsAddedByUser(Guid userId, PaginationParameters paging);

    Task<Text> GetTextById(int textId, PaginationParameters paging);
}
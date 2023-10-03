using Parcorpus.Core.Models;

namespace Parcorpus.Core.Interfaces;

public interface ILanguageRepository
{
    Task<Paged<Concordance>> GetConcordance(Word word, Language desiredLanguage, Filter filter, PaginationParameters paging);
    
    Task<Paged<Text>> GetTextsAddedByUser(Guid userId, PaginationParameters paging);

    Task AddAlignedText(Guid userId, Text alignedText);
    
    Task DeleteTextById(Guid userId, int textId);

    Task<bool> TextExists(MetaAnnotation metaAnnotation, Language sourceLanguage, Language targetLanguage);

    Task<PagedText> GetTextById(int textId, PaginationParameters paging);
}
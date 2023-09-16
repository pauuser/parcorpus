using Parcorpus.Core.Models;

namespace Parcorpus.Core.Interfaces;

public interface ILanguageRepository
{
    Task<List<Concordance>> GetConcordance(Word word, Language desiredLanguage, Filter filter);
    
    Task<List<Text>> GetTextsAddedByUser(Guid userId);

    Task AddAlignedText(Guid userId, Text alignedText);
    
    Task DeleteTextById(Guid userId, int textId);

    Task<bool> TextExists(MetaAnnotation metaAnnotation, Language sourceLanguage, Language targetLanguage);

    Task<Text> GetTextById(int textId);
}
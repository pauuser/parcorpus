using Parcorpus.Core.Models;

namespace Parcorpus.Core.Interfaces;

public interface ILanguageService
{
    Task<List<Concordance>> GetConcordance(Guid userId, ConcordanceQuery query);

    Task UploadText(Guid userId, BiText text);

    Task DeleteText(Guid userId, int textId);

    Task<List<Text>> GetTextsAddedByUser(Guid userId);

    Task<Text> GetTextById(int textId);
}
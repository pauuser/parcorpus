using Parcorpus.Core.Models;

namespace Parcorpus.Core.Interfaces;

public interface ISentenceAligner
{
    Task<List<Dictionary<string, string>>> AlignSentences(Dictionary<Language, string> languageData);
}
using Parcorpus.Core.Models;

namespace Parcorpus.Core.Interfaces;

public interface IWordAligner
{
    Task<List<WordCorrespondence>> AlignWords(string sourceText, string targetText, Language sourceLanguage, Language targetLanguage);
}
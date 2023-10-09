using Parcorpus.Core.Models;

namespace Parcorpus.UnitTests.Common.Factories.CoreModels;

public static class ConcordanceFactory
{
    public static Concordance Create(string sourceWord = "SourceWord",
        string alignedWord = "AlignedWord",
        string sourceText = "SourceText",
        string alignedTranslation = "AlignedTranslation",
        string title = "Title",
        string author = "Author",
        string source = "Source",
        int creationYear = 2023,
        DateTime? addDate = null)
    {
        return new Concordance(sourceWord, alignedWord, sourceText, alignedTranslation,
            title, author, source, creationYear, addDate ?? new DateTime(2023, 9, 1));
    }
}
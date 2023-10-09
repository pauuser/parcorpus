using Parcorpus.Core.Models;

namespace Parcorpus.UnitTests.Common.Factories.CoreModels;

public static class TextFactory
{
    private static readonly Language DefaultSourceLanguage = new(shortName: "ru", fullEnglishName: "Russian");
    private static readonly Language DefaultTargetLanguage = new(shortName: "en", fullEnglishName: "English");
    
    public static Text Create(int textId = 1, 
        string title = "Title", 
        string author = "Author", 
        string source = "Source", 
        int creationYear = 2023, 
        DateTime? addDate = null, 
        Language? sourceLanguage = null, 
        Language? targetLanguage = null,
        List<string>? genres = null,
        List<Sentence>? sentences = null,
        Guid? addedBy = null)
    {
        return new Text(textId, title, author, source, creationYear,
            addDate ?? new DateTime(2023, 9, 1),
            sourceLanguage ?? DefaultSourceLanguage,
            targetLanguage ?? DefaultTargetLanguage,
            genres ?? new(),
            sentences ?? new(),
            addedBy ?? Guid.Empty);
    }
}
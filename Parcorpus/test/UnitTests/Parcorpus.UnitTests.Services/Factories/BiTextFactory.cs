using Parcorpus.Core.Models;

namespace Parcorpus.UnitTests.Services.Factories;

public static class BiTextFactory
{
    private static readonly Language DefaultSourceLanguage = new(shortName: "ru", fullEnglishName: "Russian");
    private static readonly Language DefaultTargetLanguage = new(shortName: "en", fullEnglishName: "English");
    
    public static BiText Create(string sourceText = "Князь Андрей открыл окно",
        string targetText = "Prince Andrew opened the window",
        Language? sourceLanguage = null,
        Language? targetLanguage = null,
        MetaAnnotation? metaAnnotation = null, 
        List<string>? genres = null)
    {
        return new BiText(sourceText, targetText, 
            sourceLanguage: sourceLanguage ?? DefaultSourceLanguage, 
            targetLanguage: targetLanguage ?? DefaultTargetLanguage, 
            metaAnnotation: metaAnnotation ?? MetaAnnotationFactory.Create(), 
            genres: genres ?? new List<string>() { "Novel" });
    }
}
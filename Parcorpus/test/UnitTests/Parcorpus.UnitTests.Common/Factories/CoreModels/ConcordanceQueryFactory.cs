using Parcorpus.Core.Models;

namespace Parcorpus.UnitTests.Common.Factories.CoreModels;

public static class ConcordanceQueryFactory
{
    private static readonly Language DefaultSourceLanguage = new(shortName: "ru", fullEnglishName: "Russian");
    private static readonly Language DefaultTargetLanguage = new(shortName: "en", fullEnglishName: "English");
    
    public static ConcordanceQuery Create(string sourceWord = "Яблоко",
        Language? sourceLanguage = null,
        Language? destinationLanguage = null,
        Filter? filters = null)
    {
        var word = new Word(sourceWord, sourceLanguage ?? DefaultSourceLanguage);

        return new ConcordanceQuery(sourceWord: word,
            destinationLanguage: destinationLanguage ?? DefaultTargetLanguage,
            filters: filters ?? new());
    }
}
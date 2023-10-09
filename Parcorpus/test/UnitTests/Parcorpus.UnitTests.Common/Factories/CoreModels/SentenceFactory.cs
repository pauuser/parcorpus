using Parcorpus.Core.Models;

namespace Parcorpus.UnitTests.Common.Factories.CoreModels;

public static class SentenceFactory
{
    private static readonly Language DefaultSourceLanguage = new(shortName: "ru", fullEnglishName: "Russian");
    private static readonly Language DefaultTargetLanguage = new(shortName: "en", fullEnglishName: "English");
    
    public static Sentence Create(string sourceSentence = "", string targetSentence = "", 
        Language? sourceLanguage = null, Language? targetLanguage = null,
        List<KeyValuePair<string, string>>? words = null)
    {
        var correspondences = words.Select(kvp => 
            WordCorrespondenceFactory.Create(kvp.Key, kvp.Value, 
                sourceLanguage ?? DefaultSourceLanguage, 
                targetLanguage ?? DefaultTargetLanguage)).ToList();

        return new Sentence(default, sourceSentence, targetSentence, correspondences);
    }
}
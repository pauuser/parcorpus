using Parcorpus.Core.Models;

namespace Parcorpus.UnitTests.Common.Factories.CoreModels;

public static class SentenceFactory
{
    public static Sentence Create(string sourceSentence, string targetSentence, 
        Language sourceLanguage, Language targetLanguage,
        List<KeyValuePair<string, string>> words)
    {
        var correspondences = words.Select(kvp => 
            WordCorrespondenceFactory.Create(kvp.Key, kvp.Value, sourceLanguage, targetLanguage)).ToList();

        return new Sentence(default, sourceSentence, targetSentence, correspondences);
    }
}
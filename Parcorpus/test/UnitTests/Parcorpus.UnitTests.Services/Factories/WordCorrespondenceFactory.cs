using Parcorpus.Core.Models;

namespace Parcorpus.UnitTests.Services.Factories;

public static class WordCorrespondenceFactory
{
    public static WordCorrespondence Create(string sourceWord,
        string targetWord,
        Language sourceLanguage,
        Language targetLanguage)
    {
        return new WordCorrespondence(sourceWord: new Word(sourceWord, sourceLanguage),
            alignedWord: new Word(targetWord, targetLanguage));
    }
}
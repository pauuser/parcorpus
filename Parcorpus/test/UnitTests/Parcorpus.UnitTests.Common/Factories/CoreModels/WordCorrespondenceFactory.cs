using Parcorpus.Core.Models;

namespace Parcorpus.UnitTests.Common.Factories.CoreModels;

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
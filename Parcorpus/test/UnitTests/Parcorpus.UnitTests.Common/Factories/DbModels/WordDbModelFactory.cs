using Parcorpus.DataAccess.Models;

namespace Parcorpus.UnitTests.Common.Factories.DbModels;

public static class WordDbModelFactory
{
    public static WordDbModel Create(int wordId = 1, 
        string sourceWord = "", 
        string alignedWord = "", 
        int sentence = 1,
        SentenceDbModel? sentenceModel = null)
    {
        var word = new WordDbModel(wordId, sourceWord, alignedWord, sentence)
        {
            SentenceNavigation = sentenceModel
        };

        return word;
    }
}
using Parcorpus.DataAccess.Models;

namespace Parcorpus.UnitTests.Common.Factories.DbModels;

public static class SentenceDbModelFactory
{
    public static SentenceDbModel Create(int sentenceId = default, 
        string sourceText = "apple", 
        string alignedTranslation = "яблоко", 
        int sourceTextId = 1)
    {
        return new SentenceDbModel(sentenceId, sourceText, alignedTranslation, sourceTextId);
    }
}
using Parcorpus.DataAccess.Models;

namespace Parcorpus.UnitTests.Common.Factories.DbModels;

public static class LanguagePairDbModelFactory
{
    public static LanguagePairDbModel Create(int languagePairId = 1, int fromLanguage = 1, int toLanguage = 2)
    {
        return new LanguagePairDbModel(languagePairId, fromLanguage, toLanguage);
    }
}
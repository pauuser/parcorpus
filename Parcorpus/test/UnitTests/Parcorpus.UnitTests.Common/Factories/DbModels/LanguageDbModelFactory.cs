using Parcorpus.DataAccess.Models;

namespace Parcorpus.UnitTests.Common.Factories.DbModels;

public static class LanguageDbModelFactory
{
    public static LanguageDbModel Create(int languageId = 1, string shortName = "en", string fullName = "English")
    {
        return new LanguageDbModel(languageId, shortName, fullName);
    }
}
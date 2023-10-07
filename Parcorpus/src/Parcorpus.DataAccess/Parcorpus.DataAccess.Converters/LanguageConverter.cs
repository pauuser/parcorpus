using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Models;

namespace Parcorpus.DataAccess.Converters;

public static class LanguageConverter
{
    public static LanguageDbModel ConvertAppModelToDbModel(Language language)
    {
        return new LanguageDbModel(languageId: default,
            shortName: language.ShortName,
            fullName: language.FullEnglishName);
    }

    public static Language ConvertDbModelToAppModel(LanguageDbModel languageDbModel)
    {
        return new Language(shortName: languageDbModel.ShortName,
            fullEnglishName: languageDbModel.FullName);
    }
}
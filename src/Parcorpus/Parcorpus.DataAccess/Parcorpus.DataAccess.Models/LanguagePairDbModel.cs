using Parcorpus.DB.Models;

namespace Parcorpus.DataAccess.Models;

public class LanguagePairDbModel
{
    public int LanguagePairId { get; set; }

    public int FromLanguage { get; set; }

    public int ToLanguage { get; set; }

    public virtual LanguageDbModel? FromLanguageNavigation { get; set; }

    public virtual ICollection<TextDbModel> Texts { get; set; } = new List<TextDbModel>();

    public virtual LanguageDbModel? ToLanguageNavigation { get; set; }

    public LanguagePairDbModel(int languagePairId, int fromLanguage, int toLanguage)
    {
        LanguagePairId = languagePairId;
        FromLanguage = fromLanguage;
        ToLanguage = toLanguage;
    }

    public LanguagePairDbModel()
    {
    }
}
using Parcorpus.DB.Models;

namespace Parcorpus.DataAccess.Models;

public class LanguageDbModel
{
    public int LanguageId { get; set; }

    public string ShortName { get; set; }

    public string FullName { get; set; }

    public virtual ICollection<LanguagePairDbModel> LanguagePairFromLanguageNavigations { get; set; } = new List<LanguagePairDbModel>();

    public virtual ICollection<LanguagePairDbModel> LanguagePairToLanguageNavigations { get; set; } = new List<LanguagePairDbModel>();

    public virtual ICollection<UserDbModel> Users { get; set; } = new List<UserDbModel>();

    public LanguageDbModel(int languageId, string shortName, string fullName)
    {
        LanguageId = languageId;
        ShortName = shortName;
        FullName = fullName;
    }

    public LanguageDbModel()
    {
    }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace Parcorpus.DataAccess.Models;

public class UserDbModel
{
    public Guid UserId { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }

    public string Email { get; set; }

    public int Country { get; set; }

    public int NativeLanguage { get; set; }

    public string PasswordHash { get; set; }

    [ForeignKey("Country")]
    public virtual CountryDbModel CountryNavigation { get; set; }

    [ForeignKey("NativeLanguage")]
    public virtual LanguageDbModel NativeLanguageNavigation { get; set; }

    public virtual CredentialDbModel CredentialNavigation { get; set; }

    public virtual ICollection<JobDbModel> JobNavigation { get; set; } = new List<JobDbModel>();

    public virtual ICollection<TextDbModel> TextsNavigation { get; set; } = new List<TextDbModel>();

    public virtual ICollection<SearchHistoryDbModel> SearchHistoryNavigation { get; set; } = new List<SearchHistoryDbModel>();

    public UserDbModel(Guid userId, 
        string name, 
        string surname, 
        string email, 
        int country, 
        int nativeLanguage,
        string passwordHash)
    {
        UserId = userId;
        Name = name;
        Surname = surname;
        Email = email;
        Country = country;
        NativeLanguage = nativeLanguage;
        PasswordHash = passwordHash;
    }

    public UserDbModel()
    {
    }
}
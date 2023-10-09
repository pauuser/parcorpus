using System.ComponentModel.DataAnnotations.Schema;

namespace Parcorpus.DataAccess.Models;

public class UserDbModel : IEquatable<UserDbModel>
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

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(UserDbModel? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return UserId.Equals(other.UserId) && 
               Name == other.Name && 
               Surname == other.Surname && 
               Email == other.Email && 
               Country == other.Country && 
               NativeLanguage == other.NativeLanguage && 
               PasswordHash == other.PasswordHash && 
               CountryNavigation.Equals(other.CountryNavigation) && 
               NativeLanguageNavigation.Equals(other.NativeLanguageNavigation) && 
               CredentialNavigation.Equals(other.CredentialNavigation) && 
               JobNavigation.Equals(other.JobNavigation) && 
               TextsNavigation.Equals(other.TextsNavigation) && 
               SearchHistoryNavigation.Equals(other.SearchHistoryNavigation);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(UserId);
        hashCode.Add(Name);
        hashCode.Add(Surname);
        hashCode.Add(Email);
        hashCode.Add(Country);
        hashCode.Add(NativeLanguage);
        hashCode.Add(PasswordHash);
        hashCode.Add(CountryNavigation);
        hashCode.Add(NativeLanguageNavigation);
        hashCode.Add(CredentialNavigation);
        hashCode.Add(JobNavigation);
        hashCode.Add(TextsNavigation);
        hashCode.Add(SearchHistoryNavigation);
        return hashCode.ToHashCode();
    }
}
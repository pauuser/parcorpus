using System.Net.Mail;

namespace Parcorpus.Core.Models;

public sealed class User : IEquatable<User>
{
    public Guid UserId { get; set; }

    public UserName Name { get; set; }

    public MailAddress Email { get; set; }
    
    public Country Country { get; set; }
    
    public Language NativeLanguage { get; set; }

    public string PasswordHash { get; set; }

    public User(Guid userId, 
        string name, 
        string surname, 
        string email, 
        string countryName, 
        Language language,
        string passwordHash)
    {
        UserId = userId;
        Name = new UserName(name, surname);
        Email = new MailAddress(email);
        Country = new Country(countryName);
        NativeLanguage = language;
        PasswordHash = passwordHash;
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(User? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return UserId.Equals(other.UserId) && 
               Name.Equals(other.Name) && 
               Email.Equals(other.Email) && 
               Country.Equals(other.Country) && 
               NativeLanguage.Equals(other.NativeLanguage) && 
               PasswordHash == other.PasswordHash;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(UserId, Name, Email, Country, NativeLanguage, PasswordHash);
    }
}
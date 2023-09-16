using System.Net.Mail;

namespace Parcorpus.Core.Models;

public sealed class User
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
}
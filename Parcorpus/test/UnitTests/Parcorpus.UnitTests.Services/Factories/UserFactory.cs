using Parcorpus.Core.Models;

namespace Parcorpus.UnitTests.Services.Factories;

public static class UserFactory
{
    private static readonly Language DefaultLanguage = new(shortName: "ru", fullEnglishName: "Russian");
    
    public static User Create(Guid? userId = null, 
        string name = "Ivan", 
        string surname = "Ivanov", 
        string email = "inbox@mail.ru", 
        string countryName = "Russia", 
        Language? language = null,
        string passwordHash = "")
    {
        return new User(userId: userId ?? Guid.Empty,
            name: name,
            surname: surname,
            email: email,
            countryName: countryName,
            language: language ?? DefaultLanguage,
            passwordHash: passwordHash);
    }
}
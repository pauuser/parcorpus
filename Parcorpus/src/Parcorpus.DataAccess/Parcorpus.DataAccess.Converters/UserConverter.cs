using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Models;

namespace Parcorpus.DataAccess.Converters;

public static class UserConverter
{
    public static User? ConvertDbModelToAppModel(UserDbModel? user)
    {
        if (user is null)
            return null;
        
        return new(userId: user.UserId,
            name: user.Name,
            surname: user.Surname,
            email: user.Email,
            countryName: user.CountryNavigation.Name,
            language: LanguageConverter.ConvertDbModelToAppModel(user.NativeLanguageNavigation),
            passwordHash: user.PasswordHash);
    }
}
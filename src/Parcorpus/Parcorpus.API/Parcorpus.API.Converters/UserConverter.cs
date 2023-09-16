using Parcorpus.API.Dto;
using Parcorpus.Core.Models;
using Parcorpus.Services.Helpers;

namespace Parcorpus.API.Converters;

public static class UserConverter
{
    public static User ConvertDtoToAppModel(UserRegistrationDto user, Language userNativeLanguage, string password)
    {
        return new User(userId: Guid.Empty,
            name: user.Name,
            surname: user.Surname,
            email: user.Email,
            countryName: user.CountryName,
            language: userNativeLanguage,
            passwordHash: HashHelper.Sha256Hash(password));
    }

    public static UserDto ConvertAppModelToDto(User user)
    {
        return new UserDto(userId: user.UserId,
            name: user.Name.Name,
            surname: user.Name.Surname,
            email: user.Email.Address,
            countryName: user.Country.CountryName,
            nativeLanguageShortName: user.NativeLanguage.ShortName);
    }
}
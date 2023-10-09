using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Models;

namespace Parcorpus.UnitTests.Common.Factories.DbModels;

public static class UserDbModelFactory
{
    public static UserDbModel Create(Guid? userId = null,
        string name = "Ivan",
        string surname = "Vasiliev",
        string email = "inbox@mail.ru",
        int country = 1,
        int nativeLanguage = 1,
        string passwordHash = "")
    {
        return new UserDbModel(userId ?? Guid.Empty,
            name, surname, email, 1, nativeLanguage, passwordHash);
    }

    public static UserDbModel Create(User user, int country = 1, int nativeLanguage = 1)
    {
        return new UserDbModel(user.UserId, user.Name.Name, user.Name.Surname,
            user.Email.Address, country, nativeLanguage, user.PasswordHash);
    }
}
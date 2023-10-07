using Parcorpus.Core.Models;

namespace Parcorpus.Core.Interfaces;

public interface IUserRepository
{
    Task<User> GetUserById(Guid userId);

    Task<User> RegisterUser(User newUser);

    Task DeleteUser(Guid userId);

    Task<User?> GetUserByEmail(string email);

    Task<User> UpdateUser(User newUser);
}
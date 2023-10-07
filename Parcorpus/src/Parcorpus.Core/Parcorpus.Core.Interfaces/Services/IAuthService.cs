using Parcorpus.Core.Models;

namespace Parcorpus.Core.Interfaces;

public interface IAuthService
{
    Task<TokenPair> RegisterUser(User newUser);

    Task<TokenPair> LoginUser(string email, string password);

    Task<TokenPair> RefreshToken(string jwt, string refreshToken);
}
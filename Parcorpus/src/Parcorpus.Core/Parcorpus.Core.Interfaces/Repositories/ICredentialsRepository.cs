using Parcorpus.Core.Models;

namespace Parcorpus.Core.Interfaces;

public interface ICredentialsRepository
{
    Task<Credential> GetCredentials(Guid userId);

    Task<Credential> CreateCredentials(Credential credential);

    Task<Credential> UpdateRefreshToken(Guid userId, string newRefreshToken, DateTime expiresAtUtc);
}
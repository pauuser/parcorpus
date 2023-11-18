using Parcorpus.DataAccess.Models;

namespace Parcorpus.UnitTests.Common.Factories.DbModels;

public static class CredentialsDbModelFactory
{
    public static CredentialDbModel Create(int credentialId = default, 
        Guid? userId = null, 
        string refreshToken = "1234567890", 
        DateTime? tokenExpiresAtUtc = null)
    {
        return new CredentialDbModel(credentialId, userId ?? Guid.NewGuid(), refreshToken,
            tokenExpiresAtUtc ?? DateTime.UtcNow + TimeSpan.FromDays(1));
    }
}
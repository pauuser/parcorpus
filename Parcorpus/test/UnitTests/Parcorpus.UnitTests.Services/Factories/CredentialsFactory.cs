using Parcorpus.Core.Models;

namespace Parcorpus.UnitTests.Services.Factories;

public static class CredentialsFactory
{
    public static Credential Create(int credentialId = 0,
        Guid? userId = null,
        string refreshToken = "",
        DateTime? tokenExpiresAtUtc = null)
    {
        return new Credential(credentialId, 
            userId ?? Guid.Empty, 
            refreshToken, 
            tokenExpiresAtUtc ?? DateTime.UtcNow + TimeSpan.FromDays(1));
    }
}
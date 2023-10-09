using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Models;

namespace Parcorpus.UnitTests.Common.Factories.CoreModels;

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

    public static Credential Create(CredentialDbModel dbModel)
    {
        return new Credential(dbModel.CredentialId, dbModel.UserId, dbModel.RefreshToken, dbModel.TokenExpiresAtUtc);
    }
}
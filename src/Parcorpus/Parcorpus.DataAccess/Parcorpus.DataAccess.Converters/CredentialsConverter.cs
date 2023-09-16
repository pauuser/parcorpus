using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Models;

namespace Parcorpus.DataAccess.Converters;

public static class CredentialsConverter
{
    public static Credential ConvertDbModelToAppModel(CredentialDbModel credential)
    {
        return new Credential(credential.CredentialId, credential.UserId, credential.RefreshToken, credential.TokenExpiresAtUtc);
    }
    
    public static CredentialDbModel ConvertAppModelToDbModel(Credential credential)
    {
        return new CredentialDbModel(credential.CredentialId, credential.UserId, credential.RefreshToken, credential.TokenExpiresAtUtc);
    }
}
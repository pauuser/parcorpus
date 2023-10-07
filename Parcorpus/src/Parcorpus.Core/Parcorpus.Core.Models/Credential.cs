namespace Parcorpus.Core.Models;

public class Credential
{
    public int CredentialId { get; set; }

    public Guid UserId { get; set; }

    public string RefreshToken { get; set; }

    public DateTime TokenExpiresAtUtc { get; set; }

    public Credential(int credentialId, Guid userId, string refreshToken, DateTime tokenExpiresAtUtc)
    {
        CredentialId = credentialId;
        UserId = userId;
        RefreshToken = refreshToken;
        TokenExpiresAtUtc = tokenExpiresAtUtc;
    }
}
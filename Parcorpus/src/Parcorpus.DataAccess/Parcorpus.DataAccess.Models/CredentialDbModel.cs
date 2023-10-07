using System.ComponentModel.DataAnnotations.Schema;

namespace Parcorpus.DataAccess.Models;

public class CredentialDbModel
{
    public int CredentialId { get; set; }

    public Guid UserId { get; set; }

    public string RefreshToken { get; set; }
    
    public DateTime TokenExpiresAtUtc { get; set; }

    [ForeignKey("UserId")]
    public virtual UserDbModel UserNavigation { get; set; }

    public CredentialDbModel(int credentialId, Guid userId, string refreshToken, DateTime tokenExpiresAtUtc)
    {
        CredentialId = credentialId;
        UserId = userId;
        RefreshToken = refreshToken;
        TokenExpiresAtUtc = tokenExpiresAtUtc;
    }
}
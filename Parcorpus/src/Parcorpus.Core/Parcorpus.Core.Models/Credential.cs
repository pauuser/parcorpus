namespace Parcorpus.Core.Models;

public class Credential : IEquatable<Credential>
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

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(Credential? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return CredentialId == other.CredentialId && 
               UserId.Equals(other.UserId) && 
               RefreshToken == other.RefreshToken && 
               TokenExpiresAtUtc.Equals(other.TokenExpiresAtUtc);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CredentialId, UserId, RefreshToken, TokenExpiresAtUtc);
    }
}
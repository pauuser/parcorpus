namespace Parcorpus.Core.Models;

public sealed class TokenPair
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public TokenPair(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}
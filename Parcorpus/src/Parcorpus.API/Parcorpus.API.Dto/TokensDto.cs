using System.Text.Json.Serialization;

namespace Parcorpus.API.Dto;

/// <summary>
/// Token pair DTO
/// </summary>
public sealed class TokensDto
{
    /// <summary>
    /// Access token
    /// </summary>
    /// <example>12j5p953gym42592h3u32y21100003</example>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    /// <summary>
    /// Refresh token
    /// </summary>
    /// <example>185930@4842004T422KK5330023456</example>
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="accessToken">Access token</param>
    /// <param name="refreshToken">Refresh token</param>
    public TokensDto(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}
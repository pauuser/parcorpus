using System.Text.Json.Serialization;

namespace Parcorpus.API.Dto;

/// <summary>
/// Login DTO
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Email
    /// </summary>
    /// <example>inbox@mail.ru</example>
    [JsonPropertyName("email")]
    public string Email { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    /// <example>LuckyHamster25</example>
    [JsonPropertyName("password")]
    public string Password { get; set; }
}
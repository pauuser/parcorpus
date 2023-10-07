using System.Text.Json.Serialization;

namespace Parcorpus.API.Dto;

/// <summary>
/// Model for user registration
/// </summary>
public class UserRegistrationDto
{
    /// <summary>
    /// User's name
    /// </summary>
    /// <example>Pavel</example>
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    /// <summary>
    /// User's surname
    /// </summary>
    /// <example>Ivanov</example>
    [JsonPropertyName("surname")]
    public string Surname { get; set; }
    
    /// <summary>
    /// User's email
    /// </summary>
    /// <example>pauuser.work@gmail.com</example>
    [JsonPropertyName("email")]
    public string Email { get; set; }
    
    /// <summary>
    /// User's country name
    /// </summary>
    /// <example>Russian Federation</example>
    [JsonPropertyName("country_name")]
    public string CountryName { get; set; }
    
    /// <summary>
    /// User's language short name
    /// </summary>
    /// <example>ru</example>
    [JsonPropertyName("language_short_name")]
    public string LanguageShortName { get; set; }

    /// <summary>
    /// User's password
    /// </summary>
    /// <example>LuckyHamster24</example>
    [JsonPropertyName("password")]
    public string Password { get; set; }
}
using System.Text.Json.Serialization;

namespace Parcorpus.API.Dto;

/// <summary>
/// User DTO model
/// </summary>
public class UserDto
{
    /// <summary>
    /// Id (GUID)
    /// </summary>
    /// <example>42594DCD-913F-473F-BDD3-DBFDDC075C8B</example>
    [JsonPropertyName("user_id")]
    public Guid UserId { get; set; }

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
    /// <example>inbox@mail.ru</example>
    [JsonPropertyName("email")]
    public string Email { get; set; }
    
    /// <summary>
    /// User's country name
    /// </summary>
    /// <remarks>This should be full country name in English!</remarks>
    /// <example>Russian Federation</example>
    [JsonPropertyName("country_name")]
    public string CountryName { get; set; }
    
    /// <summary>
    /// Language code
    /// </summary>
    /// <remarks>Short name only: "en", "ru", etc</remarks>
    /// <example>ru</example>
    [JsonPropertyName("native_language_short_name")]
    public string NativeLanguageShortName { get; set; }

    /// <summary>
    /// User DTO constructor
    /// </summary>
    /// <param name="userId">user id</param>
    /// <param name="name">name</param>
    /// <param name="surname">surname</param>
    /// <param name="email">email</param>
    /// <param name="countryName">country name</param>
    /// <param name="nativeLanguageShortName">language short name</param>
    public UserDto(Guid userId, 
        string name, 
        string surname, 
        string email, 
        string countryName, 
        string nativeLanguageShortName)
    {
        UserId = userId;
        Name = name;
        Surname = surname;
        Email = email;
        CountryName = countryName;
        NativeLanguageShortName = nativeLanguageShortName;
    }
}
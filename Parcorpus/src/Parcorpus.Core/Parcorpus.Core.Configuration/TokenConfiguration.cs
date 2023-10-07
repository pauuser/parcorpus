namespace Parcorpus.Core.Configuration;

public class TokenConfiguration
{
    public const string ConfigurationSectionName = "TokenConfiguration";

    public string Issuer { get; set; }

    public string Audience { get; set; }

    public string Key { get; set; }

    public TimeSpan ExpiresIn { get; set; }

    public TimeSpan RefreshTokenExpiresIn { get; set; }
}
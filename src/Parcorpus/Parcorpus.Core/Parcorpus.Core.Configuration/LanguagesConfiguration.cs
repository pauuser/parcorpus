namespace Parcorpus.Core.Configuration;

public class LanguagesConfiguration
{
    public const string ConfigurationSectionName = "LanguagesConfiguration";
    
    public Dictionary<string, string> LanguagesForms { get; set; }
}
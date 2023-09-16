namespace Parcorpus.Core.Configuration;

public class CacheConfiguration
{
    public const string ConfigurationSectionName = "CacheConfiguration";

    public int ConcordanceExpirationMinutes { get; set; }
}
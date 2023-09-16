namespace Parcorpus.Core.Configuration;

public sealed class WebAlignerConfiguration
{
    public const string ConfigurationSectionName = "WebAlignerConfiguration"; 
    
    public string DefaultAligner { get; set; }

    public string Server { get; set; }

    public string Path { get; set; }

    public Dictionary<string, string> Sitemap { get; set; }
}
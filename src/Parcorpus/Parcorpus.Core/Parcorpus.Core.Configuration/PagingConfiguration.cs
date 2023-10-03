namespace Parcorpus.Core.Configuration;

public class PagingConfiguration
{
    public const string ConfigurationSectionName = "PagingConfiguration";
    
    public int MinPageSize { get; set; }
    
    public int MaxPageSize { get; set; }
}
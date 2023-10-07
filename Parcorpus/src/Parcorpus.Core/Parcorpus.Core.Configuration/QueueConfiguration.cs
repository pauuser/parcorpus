namespace Parcorpus.Core.Configuration;

public class QueueConfiguration
{
    public const string ConfigurationSectionName = "QueueConfiguration";
    
    public string ConnectionString { get; set; }

    public string QueueName { get; set; }
}
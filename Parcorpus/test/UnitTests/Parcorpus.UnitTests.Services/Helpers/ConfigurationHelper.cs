using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using IConfiguration = Castle.Core.Configuration.IConfiguration;

namespace Parcorpus.UnitTests.Services.Helpers;

public static class ConfigurationHelper
{
    private static readonly IConfigurationRoot Configuration;

    static ConfigurationHelper()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables() 
            .Build();
    }
    
    public static IOptions<T> InitConfiguration<T>() where T: class, new()
    {
        var section = Configuration.GetSection(typeof(T).Name);
        
        var config = new T();
        section.Bind(config);
        var options = Options.Create(config);
        
        return options;
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Parcorpus.Core.Configuration;
using Parcorpus.DataAccess.Context.Jobs;

namespace Parcorpus.DataAccess.Context.Extensions;

public static class ParcorpusDbContextExtension
{
    public static IServiceCollection ConnectToDatabase(this IServiceCollection serviceCollection, string connectionString)
    {
        serviceCollection.AddOptions<CacheConfiguration>()
            .BindConfiguration(CacheConfiguration.ConfigurationSectionName);
        
        serviceCollection.AddDbContext<ParcorpusDbContext>(options => options.UseNpgsql(connectionString))
            .Migrate<ParcorpusDbContext>();
        
        serviceCollection.AddMemoryCache();
        
        serviceCollection.AddHostedService<DbContextStartupBackgroundTask>();
        
        return serviceCollection;
    }
}
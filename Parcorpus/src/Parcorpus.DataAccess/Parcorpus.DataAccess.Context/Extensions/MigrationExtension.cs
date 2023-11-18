using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Parcorpus.DataAccess.Context.Extensions;

public static class MigrationExtension
{
    public static IApplicationBuilder Migrate<TContext>(this IApplicationBuilder builder) where TContext : DbContext
    {
        var scope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        context.Database.Migrate();

        return builder;
    }
}
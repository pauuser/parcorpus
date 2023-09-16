using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace Parcorpus.API.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        var nlogConfig = Path.Combine(AppContext.BaseDirectory, "NLog.config");
        NLogBuilder.ConfigureNLog(nlogConfig).GetCurrentClassLogger();
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
        builder.Host.UseNLog();
        builder.Services.AddLogging();

        return builder;
    }
}
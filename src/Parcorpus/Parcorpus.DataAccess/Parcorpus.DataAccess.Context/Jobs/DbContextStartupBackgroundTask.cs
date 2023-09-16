using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Parcorpus.Core.Exceptions;
using Parcorpus.DataAccess.Models;

namespace Parcorpus.DataAccess.Context.Jobs;

public class DbContextStartupBackgroundTask : BackgroundService
{
    private readonly ILogger<DbContextStartupBackgroundTask> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DbContextStartupBackgroundTask(ILogger<DbContextStartupBackgroundTask> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ParcorpusDbContext>();

            var countriesCount = await context.Countries.CountAsync();
            var languagesCount = await context.Languages.CountAsync();

            if (countriesCount == 0)
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Countries.csv");
                var countries = File.ReadAllLines(path)
                    .Skip(1)
                    .Select(c =>
                    {
                        var splitString = c.Split(',');
                        return new CountryDbModel(0, splitString[1]);
                    })
                    .ToList();
                await context.Countries.AddRangeAsync(countries);
                await context.SaveChangesAsync();
                _logger.LogInformation("Imported countries into database");
            }

            if (languagesCount == 0)
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Languages.csv");
                var languages = File.ReadAllLines(path)
                    .Skip(1)
                    .Select(c =>
                    {
                        var splitString = c.Split(',');
                        return new LanguageDbModel(0, splitString[1], splitString[2]);
                    })
                    .ToList();
                await context.Languages.AddRangeAsync(languages);
                await context.SaveChangesAsync();
                _logger.LogInformation("Imported languages into database");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error at start job. {message}", ex.Message);
            throw new StartupJobException($"Error at start job. {ex.Message}");
        }
    }
}
using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Parcorpus.Core.Configuration;
using Parcorpus.DataAccess.Context;
using Respawn;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Parcorpus.E2ETests.Fixtures;

public class ParcorpusApplicationFactory: WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("TEST_parcorpusdb")
        .WithCleanUp(true)
        .Build();

    private readonly RabbitMqContainer _broker = new RabbitMqBuilder()
        .Build();

    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;

    public HttpClient HttpClient { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ParcorpusDbContext>));
            if (descriptor != null) 
                services.Remove(descriptor);
            
            services.AddDbContext<ParcorpusDbContext>(options => options.UseNpgsql(_dbContainer.GetConnectionString()));
            
            var brokerConfiguration = new Dictionary<string, string>
            {
                { "ConnectionString", _broker.GetConnectionString() },
                { "QueueName", "TEST_queue" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(brokerConfiguration)
                .Build();

            services.Configure<QueueConfiguration>(configuration);
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _broker.StartAsync();
        
        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());

        HttpClient = CreateClient();

        await InitRespawner();
    }

    private async Task InitRespawner()
    {
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions()
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "public" }
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await _broker.DisposeAsync();
    }
}
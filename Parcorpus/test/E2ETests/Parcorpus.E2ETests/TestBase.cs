using FlueFlame.AspNetCore;
using FlueFlame.Http.Host;
using Microsoft.AspNetCore.TestHost;
using Parcorpus.E2ETests.Fixtures;
using Parcorpus.E2ETests.Seed;

namespace Parcorpus.E2ETests;

public class TestBase : IAsyncLifetime
{
    protected IFlueFlameHttpHost HttpHost { get; private set; }
    protected IServiceProvider ServiceProvider { get; private set; }
    protected TestServer TestServer { get; private set; }
    
    private ParcorpusApplicationFactory _factory;

    public async Task InitializeAsync()
    {
        _factory = new ParcorpusApplicationFactory();
        
        await _factory.InitializeAsync();
        
        TestServer = _factory.Server;
        ServiceProvider = _factory.Services;

        await DataSeeder.PopulateDatabase(_factory.Services);
        
        var builder = FlueFlameAspNetBuilder.CreateDefaultBuilder(_factory)
            .ConfigureHttpClient(c =>
            {
            });

        HttpHost = builder.BuildHttpHost(b =>
        {
            b.UseTextJsonSerializer();
            b.ConfigureHttpClient(_ => { });
        });
    }

    public async Task DisposeAsync()
    {
        await _factory.ResetDatabaseAsync();
        await _factory.DisposeAsync();
    }
}
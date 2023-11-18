using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Parcorpus.API.Dto;
using Parcorpus.IntegrationTests.Common;
using Parcorpus.IntegrationTests.Fixtures;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Parcorpus.IntegrationTests.Tests;

public class UserControllerIntegrationTests : IClassFixture<ParcorpusApplicationFactory>, IAsyncLifetime
{
    private readonly ParcorpusApplicationFactory _factory;
    private readonly HttpClient _client;

    public UserControllerIntegrationTests(ParcorpusApplicationFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _client = _factory.HttpClient;
    }

    [Fact]
    public async Task MeOkTest()
    {
        // Arrange
        var tokens = await _client.GetDefaultUserTokens();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        // Act
        var response = await _client.GetAsync("/api/v1/users/me");

        // Assert
        Assert.True(response.IsSuccessStatusCode);

        var user = JsonSerializer.Deserialize<UserDto>(await response.Content.ReadAsStringAsync());
        Assert.Equal("Ivan", user.Name);
        Assert.Equal("Vasiliev", user.Surname);
        Assert.Equal("inbox@mail.ru", user.Email);
        Assert.Equal("Afghanistan", user.CountryName);
    }
    
    [Fact]
    public async Task MeUnauthorizedTest()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/users/me");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task PatchMeOkTest()
    {
        // Arrange
        var tokens = await _client.GetDefaultUserTokens();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        var newName = "Pavel";
        var request = new[]
        {
            new Dictionary<string, string>
            {
                { "op", "add" },
                { "path", "/name" },
                { "value", newName }
            }
        };
        var payload = JsonConvert.SerializeObject(request);
        var content = new StringContent(payload, Encoding.UTF8, "application/json-patch+json");
        
        // Act
        var response = await _client.PatchAsync("/api/v1/users/me", content);

        // Assert
        Assert.True(response.IsSuccessStatusCode);

        var user = JsonSerializer.Deserialize<UserDto>(await response.Content.ReadAsStringAsync());
        Assert.Equal(newName, user.Name);
        Assert.Equal("Vasiliev", user.Surname);
        Assert.Equal("inbox@mail.ru", user.Email);
        Assert.Equal("Afghanistan", user.CountryName);
    }
    
    [Fact]
    public async Task PatchMeUnauthorizedTest()
    {
        // Arrange
        var newName = "Pavel";
        var request = new[]
        {
            new Dictionary<string, string>
            {
                { "op", "add" },
                { "path", "/name" },
                { "value", newName }
            }
        };
        var payload = JsonConvert.SerializeObject(request);
        var content = new StringContent(payload, Encoding.UTF8, "application/json-patch+json");
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "123");
        
        // Act
        var response = await _client.PatchAsync("/api/v1/users/me", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task PatchMeBadRequestTest()
    {
        // Arrange
        var newLanguage = "abobus";
        var request = new[]
        {
            new Dictionary<string, string>
            {
                { "op", "add" },
                { "path", "/NativeLanguageShortName" },
                { "value", newLanguage }
            }
        };
        var payload = JsonConvert.SerializeObject(request);
        var content = new StringContent(payload, Encoding.UTF8, "application/json-patch+json");
        
        var tokens = await _client.GetDefaultUserTokens();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        
        // Act
        var response = await _client.PatchAsync("/api/v1/users/me", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task HistoryOkTest()
    {
        // Arrange
        var tokens = await _client.GetDefaultUserTokens();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        
        // Act
        var response = await _client.GetAsync("/api/v1/users/history?page=1&page_size=10");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }
    
    [Fact]
    public async Task HistoryUnauthorizedTest()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/users/history?page=1&page_size=10");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    public async Task InitializeAsync()
    {
        await DataInitializer.PopulateDatabase(_factory.Services);
    }

    public Task DisposeAsync()
    {
        return _factory.ResetDatabaseAsync();
    }
}
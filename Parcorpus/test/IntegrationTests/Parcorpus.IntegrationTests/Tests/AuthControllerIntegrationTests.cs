using System.Net;
using System.Text;
using Newtonsoft.Json;
using Parcorpus.API.Dto;
using Parcorpus.IntegrationTests.Common;
using Parcorpus.IntegrationTests.Fixtures;

namespace Parcorpus.IntegrationTests.Tests;

public class AuthControllerIntegrationTests : IClassFixture<ParcorpusApplicationFactory>, IAsyncLifetime
{
    private readonly ParcorpusApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthControllerIntegrationTests(ParcorpusApplicationFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _client = _factory.HttpClient;
    }

    [Fact]
    public async Task RegisterOkTest()
    {
        // Arrange
        var request = new Dictionary<string, string>
        {
            { "name", "login" },
            { "surname", "login" },
            { "email", "login@mail.ru" },
            { "country_name", "Russian Federation" },
            { "language_short_name", "en" },
            { "password", "login" }
        };
        var payload = JsonConvert.SerializeObject(request);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        
        // Act
        var response = await _client.PostAsync("/api/v1/auth/register", content);
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }
    
    [Fact]
    public async Task RegisterConflictTest()
    {
        // Arrange
        var request = new Dictionary<string, string>
        {
            { "name", "login" },
            { "surname", "login" },
            { "email", "inbox@mail.ru" },
            { "country_name", "Russian Federation" },
            { "language_short_name", "en" },
            { "password", "login" }
        };
        var payload = JsonConvert.SerializeObject(request);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        
        // Act
        var response = await _client.PostAsync("/api/v1/auth/register", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    
    [Fact]
    public async Task LoginUserOkTest()
    {
        // Arrange
        var request = new Dictionary<string, string>
        {
            { "email", "inbox@mail.ru" },
            { "password", "123" }
        };
        var payload = JsonConvert.SerializeObject(request);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        
        // Act
        var response = await _client.PostAsync("/api/v1/auth/login", content);
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }
    
    [Fact]
    public async Task LoginUserUnauthorizedTest()
    {
        // Arrange
        var request = new Dictionary<string, string>
        {
            { "email", "inbox@mail.ru" },
            { "password", "incorrect_password" }
        };
        var payload = JsonConvert.SerializeObject(request);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        
        // Act
        var response = await _client.PostAsync("/api/v1/auth/login", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task LoginUserNotFoundTest()
    {
        // Arrange
        var request = new Dictionary<string, string>
        {
            { "email", "does_not_exist@mail.ru" },
            { "password", "123" }
        };
        var payload = JsonConvert.SerializeObject(request);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        
        // Act
        var response = await _client.PostAsync("/api/v1/auth/login", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task RefreshOkTest()
    {
        // Arrange
        var tokens = await _client.GetDefaultUserTokens(); // first we log in and get tokens
        var request = new Dictionary<string, string>
        {
            { "access_token", tokens.AccessToken },
            { "refresh_token", tokens.RefreshToken }
        };
        var payload = JsonConvert.SerializeObject(request);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        
        // Act
        var response = await _client.PostAsync("/api/v1/auth/refresh", content);
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        
        var newTokens = System.Text.Json.JsonSerializer.Deserialize<TokensDto>(await response.Content.ReadAsStringAsync())!;
        Assert.NotEqual(tokens.RefreshToken, newTokens.RefreshToken);  // checking that new credentials are given
    }
    
    [Theory]
    [InlineData(null, "invalid_refresh")]
    [InlineData("invalid_access", null)]
    [InlineData("invalid_access", "invalid_refresh")]
    public async Task RefreshUnauthorizedTest(string? accessToken, string? refreshToken)
    {
        // Arrange
        var tokens = await _client.GetDefaultUserTokens(); // first we log in and get tokens
        var request = new Dictionary<string, string>
        {
            { "access_token", accessToken ?? tokens.AccessToken },
            { "refresh_token", refreshToken ?? tokens.RefreshToken }
        };
        var payload = JsonConvert.SerializeObject(request);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        
        // Act
        var response = await _client.PostAsync("/api/v1/auth/refresh", content);
        
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
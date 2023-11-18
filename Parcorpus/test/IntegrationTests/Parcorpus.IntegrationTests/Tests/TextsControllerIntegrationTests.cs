using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Parcorpus.API.Dto;
using Parcorpus.DataAccess.Context;
using Parcorpus.DataAccess.Models;
using Parcorpus.IntegrationTests.Common;
using Parcorpus.IntegrationTests.Fixtures;

namespace Parcorpus.IntegrationTests.Tests;

public class TextsControllerIntegrationTests : IClassFixture<ParcorpusApplicationFactory>, IAsyncLifetime
{
    private readonly ParcorpusApplicationFactory _factory;
    private readonly HttpClient _client;

    public TextsControllerIntegrationTests(ParcorpusApplicationFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _client = _factory.HttpClient;
    }

    [Fact]
    public async Task GetConcordanceOkTest()
    {
        // Arrange
        var tokens = await _client.GetDefaultUserTokens();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        
        // Act
        var response = await _client.GetAsync("/api/v1/texts/concordance?Word=apple&SourceLanguageShortName=aa&TargetLanguageShortName=ab");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }
    
    [Fact]
    public async Task GetConcordanceUnauthorizedTest()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "123");
        
        // Act
        var response = await _client.GetAsync("/api/v1/texts/concordance?Word=apple&SourceLanguageShortName=aa&TargetLanguageShortName=ab");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task GetConcordanceNotFoundTest()
    {
        // Arrange
        var tokens = await _client.GetDefaultUserTokens();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        
        // Act
        var response = await _client.GetAsync("/api/v1/texts/concordance?Word=abobus&SourceLanguageShortName=aa&TargetLanguageShortName=ab");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetUserTextsOkTest()
    {
        // Arrange
        var tokens = await _client.GetDefaultUserTokens();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        
        // Act
        var response = await _client.GetAsync("/api/v1/texts/texts");

        // Assert
        Assert.True(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var texts = JsonSerializer.Deserialize<PagedDto<TextDto>>(content);
        Assert.Single(texts.Items);
    }
    
    [Fact]
    public async Task GetUserTextsUnauthorizedTest()
    {
        // Arrange
        var tokens = await _client.GetDefaultUserTokens();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "1231");
        
        // Act
        var response = await _client.GetAsync("/api/v1/texts/texts");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task GetTextByIdNotFoundTest()
    {
        // Arrange
        var tokens = await _client.GetDefaultUserTokens();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        
        // Act
        var response = await _client.GetAsync($"/api/v1/texts/{123123}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetTextByIdUnauthorizedTest()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "1233");
        
        // Act
        var response = await _client.GetAsync($"/api/v1/texts/texts/123123");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task GetTextByIdOkTest()
    {
        // Arrange
        var tokens = await _client.GetDefaultUserTokens();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        var userId = await _client.GetUserId(tokens.AccessToken);
        var text = await GetUserText(userId);
        
        // Act
        var response = await _client.GetAsync($"/api/v1/texts/texts/{text.TextId}");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }

    private async Task<TextDbModel?> GetUserText(Guid? userId = null)
    {
        userId ??= DataInitializer.DefaultUserId;
        
        using var scope = _factory.Services.GetService<IServiceScopeFactory>().CreateScope();
        var context = scope.ServiceProvider.GetService<ParcorpusDbContext>();

        var text = await context.Texts.FirstOrDefaultAsync(t => t.AddedBy == userId);

        return text;
    }

    private async Task<TextDbModel> GetTextById(int textId)
    {
        using var scope = _factory.Services.GetService<IServiceScopeFactory>().CreateScope();
        var context = scope.ServiceProvider.GetService<ParcorpusDbContext>();

        var text = await context.Texts.FirstOrDefaultAsync(t => t.TextId == textId);

        return text;
    }
    
    [Fact]
    public async Task DeleteTextByIdOkTest()
    {
        // Arrange
        var tokens = await _client.GetDefaultUserTokens();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        var userId = await _client.GetUserId(tokens.AccessToken);
        var text = await GetUserText(userId);
        
        // Act
        var response = await _client.DeleteAsync($"/api/v1/texts/texts/{text.TextId}");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        var updatedTextId = await GetTextById(text.TextId);
        Assert.Null(updatedTextId);
    }
    
    [Fact]
    public async Task DeleteTextByIdUnauthorizedTest()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "123");
        
        // Act
        var response = await _client.DeleteAsync($"/api/v1/texts/texts/123");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteTextByIdNotFoundTest()
    {
        // Arrange
        var tokens = await _client.GetDefaultUserTokens();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        
        // Act
        var response = await _client.DeleteAsync($"/api/v1/texts/texts/123123");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;
using Parcorpus.IntegrationTests.Common;
using Parcorpus.IntegrationTests.Fixtures;

namespace Parcorpus.IntegrationTests.Tests;

public class JobsControllerIntegrationTests : IClassFixture<ParcorpusApplicationFactory>, IAsyncLifetime
{
    private readonly ParcorpusApplicationFactory _factory;
    private readonly HttpClient _client;

    public JobsControllerIntegrationTests(ParcorpusApplicationFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _client = _factory.HttpClient;
    }

    [Fact]
    public async Task UploadTextOkTest()
    {
        // Arrange
        var tokens = await _client.GetDefaultUserTokens();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        
        var httpContent = new MultipartFormDataContent();

        var enFilename = "Data/en.txt";
        var ruFilename = "Data/ru.txt";
        
        var enFileContent = new ByteArrayContent(File.ReadAllBytes(enFilename));
        var ruFileContent = new ByteArrayContent(File.ReadAllBytes(ruFilename));
        enFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        ruFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        
        httpContent.Add(enFileContent, "SourceText", enFilename);
        httpContent.Add(ruFileContent, "TargetText", ruFilename);
        
        httpContent.Add(new StringContent("en"), "SourceLanguageCode");
        httpContent.Add(new StringContent("ru"), "TargetLanguageCode");
        httpContent.Add(new StringContent("Test_Title"), "Title");
        httpContent.Add(new StringContent("Test_Author"), "Author");
        httpContent.Add(new StringContent("Test_Source"), "Source");
        httpContent.Add(new StringContent("Test_Genres"), "Genres");
        
        // Act
        var response = await _client.PostAsync("/api/v1/jobs", httpContent);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }
    
    [Fact]
    public async Task UploadTextUnauthorizedTest()
    {
        // Arrange
        var tokens = await _client.GetDefaultUserTokens();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "123");
        
        var httpContent = new MultipartFormDataContent();

        var enFilename = "Data/en.txt";
        var ruFilename = "Data/ru.txt";
        
        var enFileContent = new ByteArrayContent(File.ReadAllBytes(enFilename));
        var ruFileContent = new ByteArrayContent(File.ReadAllBytes(ruFilename));
        enFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        ruFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        
        httpContent.Add(enFileContent, "SourceText", enFilename);
        httpContent.Add(ruFileContent, "TargetText", ruFilename);
        
        httpContent.Add(new StringContent("en"), "SourceLanguageCode");
        httpContent.Add(new StringContent("ru"), "TargetLanguageCode");
        httpContent.Add(new StringContent("Test_Title"), "Title");
        httpContent.Add(new StringContent("Test_Author"), "Author");
        httpContent.Add(new StringContent("Test_Source"), "Source");
        httpContent.Add(new StringContent("Test_Genres"), "Genres");
        
        // Act
        var response = await _client.PostAsync("/api/v1/jobs", httpContent);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task UploadTextBadRequestTest()
    {
        // Arrange
        var tokens = await _client.GetDefaultUserTokens();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        
        var httpContent = new MultipartFormDataContent();

        var enFilename = "Data/en.txt";
        var ruFilename = "Data/ru.txt";
        
        var enFileContent = new ByteArrayContent(File.ReadAllBytes(enFilename));
        var ruFileContent = new ByteArrayContent(File.ReadAllBytes(ruFilename));
        enFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        ruFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        
        httpContent.Add(enFileContent, "SourceText", enFilename);
        httpContent.Add(ruFileContent, "TargetText", ruFilename);
        
        httpContent.Add(new StringContent("ABOBUS"), "SourceLanguageCode");
        httpContent.Add(new StringContent("ru"), "TargetLanguageCode");
        httpContent.Add(new StringContent("Test_Title"), "Title");
        httpContent.Add(new StringContent("Test_Author"), "Author");
        httpContent.Add(new StringContent("Test_Source"), "Source");
        httpContent.Add(new StringContent("Test_Genres"), "Genres");
        
        // Act
        var response = await _client.PostAsync("/api/v1/jobs", httpContent);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetLatestJobsOkTest()
    {
        // Arrange
        var tokens = await _client.GetDefaultUserTokens();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        // Act
        var response = await _client.GetAsync("/api/v1/jobs");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }
    
    [Fact]
    public async Task GetLatestJobsUnauthorizedTest()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "123");

        // Act
        var response = await _client.GetAsync("/api/v1/jobs");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<ProgressJob> AddJob(Guid userId)
    {
        using var scope = _factory.Services.GetService<IServiceScopeFactory>().CreateScope();
        var jobService = scope.ServiceProvider.GetService<IJobService>();

        var biText = new BiText(sourceText: "",
            targetText: "",
            sourceLanguage: new Language("ru", "Russian"),
            targetLanguage: new Language("en", "English"),
            metaAnnotation: new MetaAnnotation(title: "Title", 
                author: "Author", 
                source: "Source", 
                creationYear: 2023,
                addDate: DateTime.Now),
            genres: new List<string> { "Drama" });
        var job = await jobService.UploadText(userId, biText);

        return job;
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
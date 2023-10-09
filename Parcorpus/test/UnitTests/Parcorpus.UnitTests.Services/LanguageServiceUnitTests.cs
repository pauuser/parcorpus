using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Parcorpus.Core.Configuration;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;
using Parcorpus.Services.LanguageService;
using Parcorpus.UnitTests.Common.Factories;
using Parcorpus.UnitTests.Common.Factories.CoreModels;
using Parcorpus.UnitTests.Common.Helpers;

namespace Parcorpus.UnitTests.Services;

public class LanguageServiceUnitTests
{
    private readonly ILanguageService _languageService;

    private readonly Mock<ILanguageRepository> _mockLanguageRepository = new();
    private readonly Mock<ISearchHistoryRepository> _mockSearchHistoryRepository = new();
    private readonly PagingConfiguration _pagingConfiguration;

    public LanguageServiceUnitTests()
    {
        var pagingOptions = ConfigurationHelper.InitConfiguration<PagingConfiguration>();
        _pagingConfiguration = pagingOptions.Value;

        _languageService = new LanguageService(_mockLanguageRepository.Object,
            _mockSearchHistoryRepository.Object,
            pagingOptions,
            NullLogger<LanguageService>.Instance);
    }

    [Fact]
    public async Task GetConcordanceOkTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = ConcordanceQueryFactory.Create();

        var paging = new PaginationParameters(pageNumber: 1, pageSize: 5);

        var expectedConcordance = new Paged<Concordance>(pageNumber: paging.PageNumber,
            pageSize: paging.PageSize, totalCount: paging.PageNumber!.Value * 5,
            items: Enumerable.Range(1, paging.PageSize!.Value).Select(_ => ConcordanceFactory.Create()).ToList());
        
        var searchHistory = new List<SearchHistoryRecord>();
        _mockSearchHistoryRepository.Setup(s => s.AddRecord(userId, query))
            .Returns(async (Guid id, ConcordanceQuery query) => searchHistory.Add(SearchHistoryFactory.Create(userId: id)));
        _mockLanguageRepository.Setup(s => s.GetConcordance(query.SourceWord,
                query.DestinationLanguage, query.Filters, paging))
            .ReturnsAsync(expectedConcordance);

        // Act
        var actualConcordance = await _languageService.GetConcordance(userId, query, paging);

        // Assert
        Assert.Equal(expectedConcordance, actualConcordance);
        Assert.NotEmpty(searchHistory);
    }
    
    [Fact]
    public async Task GetConcordanceEmptyTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = ConcordanceQueryFactory.Create();

        var paging = new PaginationParameters(pageNumber: 1, pageSize: 5);

        var expectedConcordance = new Paged<Concordance>(pageNumber: paging.PageNumber,
            pageSize: paging.PageSize, totalCount: paging.PageNumber!.Value * 5,
            items: new());
        
        var searchHistory = new List<SearchHistoryRecord>();
        _mockSearchHistoryRepository.Setup(s => s.AddRecord(userId, query))
            .Returns(async (Guid id, ConcordanceQuery query) => searchHistory.Add(SearchHistoryFactory.Create(userId: id)));
        _mockLanguageRepository.Setup(s => s.GetConcordance(query.SourceWord,
                query.DestinationLanguage, query.Filters, paging))
            .ReturnsAsync(expectedConcordance);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _languageService.GetConcordance(userId, query, paging));
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    public async Task GetConcordanceInvalidPagingTest(int add)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = ConcordanceQueryFactory.Create();

        var pageSize = add > 0 ? _pagingConfiguration.MaxPageSize + add : _pagingConfiguration.MinPageSize + add;
        var paging = new PaginationParameters(pageNumber: 1, pageSize: pageSize);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidPagingException>(() => _languageService.GetConcordance(userId, query, paging));
    }

    [Fact]
    public async Task DeleteTextOkTest()
    {
        // Arrange
        var textId = 0;
        var userId = Guid.NewGuid();

        var deleted = false;
        _mockLanguageRepository.Setup(s => s.DeleteTextById(userId, textId))
            .Callback(() => deleted = true);

        // Act
        await _languageService.DeleteText(userId, textId);

        // Assert
        Assert.True(deleted);
    }

    [Fact]
    public async Task DeleteTextExceptionTest()
    {
        // Arrange
        var textId = 0;
        var userId = Guid.NewGuid();

        var deleted = false;
        _mockLanguageRepository.Setup(s => s.DeleteTextById(userId, textId))
            .ThrowsAsync(new Exception());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _languageService.DeleteText(userId, textId));
    }

    [Fact]
    public async Task GetTextsAddedByUserOkTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paging = new PaginationParameters(pageNumber: 1, pageSize: 5);

        var expectedTexts = new Paged<Text>(pageNumber: paging.PageNumber,
            pageSize: paging.PageSize,
            totalCount: 20,
            items: Enumerable.Range(1, 3).Select(_ => TextFactory.Create()).ToList());

        _mockLanguageRepository.Setup(s => s.GetTextsAddedByUser(userId, paging))
            .ReturnsAsync(expectedTexts);
        
        // Act
        var texts = await _languageService.GetTextsAddedByUser(userId, paging);

        // Assert
        Assert.Equal(expectedTexts, texts);
    }
    
    [Fact]
    public async Task GetTextsAddedByUserNoPaginationTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paging = new PaginationParameters(pageNumber: null, pageSize: null);

        var expectedTexts = new Paged<Text>(pageNumber: paging.PageNumber,
            pageSize: paging.PageSize,
            totalCount: 20,
            items: Enumerable.Range(1, 3).Select(_ => TextFactory.Create()).ToList());

        _mockLanguageRepository.Setup(s => s.GetTextsAddedByUser(userId, paging))
            .ReturnsAsync(expectedTexts);
        
        // Act
        var texts = await _languageService.GetTextsAddedByUser(userId, paging);

        // Assert
        Assert.Equal(expectedTexts, texts);
    }
    
    [Fact]
    public async Task GetTextsAddedByUserNotFoundTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paging = new PaginationParameters(pageNumber: 1, pageSize: 5);

        var expectedTexts = new Paged<Text>(pageNumber: paging.PageNumber,
            pageSize: 5,
            totalCount: 0,
            items: new());

        _mockLanguageRepository.Setup(s => s.GetTextsAddedByUser(userId, paging))
            .ReturnsAsync(expectedTexts);
        
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _languageService.GetTextsAddedByUser(userId, paging));
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    public async Task GetTextsAddedByUserInvalidPagingTest(int add)
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var pageSize = add > 0 ? _pagingConfiguration.MaxPageSize + add : _pagingConfiguration.MinPageSize + add;
        var paging = new PaginationParameters(pageNumber: 1, pageSize: pageSize);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidPagingException>(() => _languageService.GetTextsAddedByUser(userId, paging));
    }

    [Fact]
    public async Task GetTextByIdOkTest()
    {
        // Arrange
        var textId = 1;
        
        var paging = new PaginationParameters(pageNumber: 1, pageSize: 5);
        var expectedText = new PagedText(sentencesPageNumber: paging.PageNumber,
            sentencesPageSize: paging.PageSize,
            sentencesTotalCount: 20,
            text: TextFactory.Create(textId: textId));

        _mockLanguageRepository.Setup(s => s.GetTextById(textId, paging))
            .ReturnsAsync(expectedText);

        // Act
        var actualText = await _languageService.GetTextById(textId, paging);

        // Assert
        Assert.Equal(expectedText, actualText);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    public async Task GetTextByIdInvalidPagingTest(int add)
    {
        // Arrange
        var textId = 1;
        
        var pageSize = add > 0 ? _pagingConfiguration.MaxPageSize + add : _pagingConfiguration.MinPageSize + add;
        var paging = new PaginationParameters(pageNumber: 1, pageSize: pageSize);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidPagingException>(() => _languageService.GetTextById(textId, paging));
    }
    
    [Fact]
    public async Task GetTextByIdNotFoundTest()
    {
        // Arrange
        var textId = 1;
        
        var paging = new PaginationParameters(pageNumber: 1, pageSize: 5);

        _mockLanguageRepository.Setup(s => s.GetTextById(textId, paging))
            .Throws(new NotFoundException());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _languageService.GetTextById(textId, paging));
    }
}
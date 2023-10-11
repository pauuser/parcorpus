using Allure.Xunit.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.EntityFrameworkCore;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Context;
using Parcorpus.DataAccess.Models;
using Parcorpus.DataAccess.Repositories;
using Parcorpus.UnitTests.Common.Factories.CoreModels;
using Parcorpus.UnitTests.Common.Factories.DbModels;
using Parcorpus.UnitTests.Common.Helpers;

namespace Parcorpus.UnitTests.DataAccess;

[AllureSuite("Repositories")]
[AllureSubSuite("SearchHistoryRepository")]
public class SearchHistoryRepositoryUnitTests
{
    private readonly ISearchHistoryRepository _searchHistoryRepository;
    private readonly Mock<ParcorpusDbContext> _context = new();

    public SearchHistoryRepositoryUnitTests()
    {
        _searchHistoryRepository =
            new SearchHistoryRepository(NullLogger<SearchHistoryRepository>.Instance, _context.Object);
    }

    [Fact]
    public async Task AddRecordOkTest()
    {
        // Arrange
        var searchHistoryId = 1;
        var userId = Guid.NewGuid();

        var user = UserDbModelFactory.Create(userId: userId);
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .ReturnsDbSet(new List<UserDbModel> { user }.AsQueryable());
        
        var internalSearchHistory = new List<SearchHistoryDbModel>();
        _context.Setup(s => s.SearchHistory.AddAsync(It.IsAny<SearchHistoryDbModel>(), It.IsAny<CancellationToken>()))
            .Returns((SearchHistoryDbModel job, CancellationToken _) =>
            {
                var entry = EntityHelper.GetMockEntityEntry(job);
                entry.Entity.SearchHistoryId = searchHistoryId;
                internalSearchHistory.Add(entry.Entity);
                
                return ValueTask.FromResult(entry);
            });

        var query = ConcordanceQueryFactory.Create();
        var searchHistory = SearchHistoryFactory.CreateFromQuery(searchHistoryId: searchHistoryId, query: query, userId: userId);
        var searchHistoryDb = SearchHistoryDbModelFactory.Create(searchHistory);
        
        // Act
        await _searchHistoryRepository.AddRecord(userId, query);

        // Assert
        Assert.NotEmpty(internalSearchHistory);
        Assert.Equal(searchHistoryDb, internalSearchHistory.First());
    }
    
    [Fact]
    public async Task AddRecordUserNotFoundTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .ReturnsDbSet(new List<UserDbModel> { }.AsQueryable());

        var query = ConcordanceQueryFactory.Create();
        
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _searchHistoryRepository.AddRecord(userId, query));
    }
    
    [Fact]
    public async Task AddRecordExceptionTest()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .Throws<Exception>();

        var query = ConcordanceQueryFactory.Create();
        
        // Act & Assert
        await Assert.ThrowsAsync<SearchHistoryRepositoryException>(() => _searchHistoryRepository.AddRecord(userId, query));
    }

    [Fact]
    public async Task GetSearchHistoryNoPaginationTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var numberOfHistory = 1;
        var paging = new PaginationParameters(pageNumber: null, pageSize: null);
        
        var expectedHistory = Enumerable.Range(1, numberOfHistory).Select(_ => SearchHistoryDbModelFactory.Create(userId: userId)).ToList();
        _context.Setup<DbSet<SearchHistoryDbModel>>(s => s.SearchHistory)
            .ReturnsDbSet(expectedHistory.AsQueryable());
        
        // Act
        var actualHistory = await _searchHistoryRepository.GetSearchHistory(userId, paging);

        // Assert
        Assert.Equal(new Paged<SearchHistoryRecord>(pageNumber: 1, 
            pageSize: numberOfHistory, 
            items: expectedHistory.Select(SearchHistoryFactory.Create).ToList(), 
            totalCount: numberOfHistory), actualHistory);
    }
    
    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 5)]
    [InlineData(1, 150)]
    [InlineData(5, 7)]
    [InlineData(14, 11)]
    public async Task GetSearchHistoryValidPaginationTest(int pageNumber, int pageSize)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var numberOfHistory = 150;
        var paging = new PaginationParameters(pageNumber: pageNumber, pageSize: pageSize);
        
        var expectedHistory = Enumerable.Range(1, numberOfHistory).Select(_ => SearchHistoryDbModelFactory.Create(userId: userId)).ToList();
        _context.Setup<DbSet<SearchHistoryDbModel>>(s => s.SearchHistory)
            .ReturnsDbSet(expectedHistory.AsQueryable());
        
        // Act
        var actualHistory = await _searchHistoryRepository.GetSearchHistory(userId, paging);

        // Assert
        Assert.Equal(new Paged<SearchHistoryRecord>(pageNumber: pageNumber, 
            pageSize: pageSize, 
            items: expectedHistory.Select(SearchHistoryFactory.Create)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList(), 
            totalCount: expectedHistory.Count), actualHistory);
    }

    [Theory]
    [InlineData(0, 1, 150)]
    [InlineData(15, 11, 150)]
    [InlineData(1, 1, 0)]
    public async Task GetSearchHistoryInvalidPagingTest(int pageNumber, int pageSize, int numberOfHistory)
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var expectedHistory = Enumerable.Range(1, numberOfHistory).Select(_ => SearchHistoryDbModelFactory.Create(userId: userId)).ToList();
        _context.Setup<DbSet<SearchHistoryDbModel>>(s => s.SearchHistory)
            .ReturnsDbSet(expectedHistory.AsQueryable());

        var paging = new PaginationParameters(pageNumber: pageNumber, pageSize: pageSize);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidPagingException>(() => _searchHistoryRepository.GetSearchHistory(userId, paging));
    }
    
    [Fact]
    public async Task GetSearchHistoryExceptionTest()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _context.Setup<DbSet<SearchHistoryDbModel>>(s => s.SearchHistory)
            .Throws<Exception>();

        var paging = new PaginationParameters(pageNumber: null, pageSize: null);
        
        // Act & Assert
        await Assert.ThrowsAsync<SearchHistoryRepositoryException>(() => _searchHistoryRepository.GetSearchHistory(userId, paging));
    }
}
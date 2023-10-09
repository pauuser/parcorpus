using System.Net.Mail;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Parcorpus.Core.Configuration;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;
using Parcorpus.Services.UserService;
using Parcorpus.UnitTests.Common.Factories;
using Parcorpus.UnitTests.Common.Factories.CoreModels;
using Parcorpus.UnitTests.Common.Helpers;

namespace Parcorpus.UnitTests.Services;

public class UserServiceUnitTests
{
    private readonly IUserService _userService;
    
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<ISearchHistoryRepository> _mockSearchHistoryRepository = new();

    private readonly PagingConfiguration _pagingConfiguration;

    public UserServiceUnitTests()
    {
        var pagingConfiguration = ConfigurationHelper.InitConfiguration<PagingConfiguration>();
        _userService = new UserService(NullLogger<UserService>.Instance,
            _mockUserRepository.Object,
            _mockSearchHistoryRepository.Object,
            pagingConfiguration);

        _pagingConfiguration = pagingConfiguration.Value;
    }

    [Fact]
    public async Task GetUserByIdOkTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = UserFactory.Create(userId);
        _mockUserRepository.Setup(s => s.GetUserById(userId))
            .ReturnsAsync(user);

        // Act
        var actualUser = await _userService.GetUserById(userId);

        // Assert
        Assert.Equal(user, actualUser);
    }
    
    [Fact]
    public async Task UpdateUserPasswordFailTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var user = UserFactory.Create(userId);
        var patchedUser = UserFactory.Create(userId);

        user.PasswordHash = "initial";
        patchedUser.PasswordHash = "patched";
        
        _mockUserRepository.Setup(s => s.UpdateUser(patchedUser))
            .Returns(() =>
            {
                user.Name.Name = patchedUser.Name.Name;
                user.Name.Surname = patchedUser.Name.Surname;
                user.Country = patchedUser.Country;
                user.NativeLanguage = patchedUser.NativeLanguage;
                
                return Task.FromResult(patchedUser);
            });

        // Act & Assert
        await Assert.ThrowsAsync<ImpossiblePatchException>(() => _userService.UpdateUser(user, patchedUser));
    }
    
    [Fact]
    public async Task UpdateUserEmailFailTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var user = UserFactory.Create(userId);
        var patchedUser = UserFactory.Create(userId);

        user.Email = new MailAddress("initial@inbox.com");
        patchedUser.Email = new MailAddress("patched@inbox.com");
        
        _mockUserRepository.Setup(s => s.UpdateUser(patchedUser))
            .Returns(() =>
            {
                user.Name.Name = patchedUser.Name.Name;
                user.Name.Surname = patchedUser.Name.Surname;
                user.Country = patchedUser.Country;
                user.NativeLanguage = patchedUser.NativeLanguage;
                
                return Task.FromResult(patchedUser);
            });

        // Act & Assert
        await Assert.ThrowsAsync<ImpossiblePatchException>(() => _userService.UpdateUser(user, patchedUser));
    }
    
    [Fact]
    public async Task UpdateUserUserIdFailTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var patchedUserId = Guid.NewGuid();
        
        var user = UserFactory.Create(userId);
        var patchedUser = UserFactory.Create(patchedUserId);
        
        _mockUserRepository.Setup(s => s.UpdateUser(patchedUser))
            .Returns(() =>
            {
                user.Name.Name = patchedUser.Name.Name;
                user.Name.Surname = patchedUser.Name.Surname;
                user.Country = patchedUser.Country;
                user.NativeLanguage = patchedUser.NativeLanguage;
                
                return Task.FromResult(patchedUser);
            });

        // Act & Assert
        await Assert.ThrowsAsync<ImpossiblePatchException>(() => _userService.UpdateUser(user, patchedUser));
    }
    
    [Fact]
    public async Task UpdateUserNotFoundFailTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var user = UserFactory.Create(userId);
        var patchedUser = UserFactory.Create(userId);

        _mockUserRepository.Setup(s => s.UpdateUser(patchedUser))
            .ThrowsAsync(new NotFoundException());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.UpdateUser(user, patchedUser));
    }
    
    [Fact]
    public async Task GetUserSearchHistoryOkTest()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var pagination = new PaginationParameters(pageNumber: 1, pageSize: 5);
        var expectedHistory = new Paged<SearchHistoryRecord>(pageNumber: pagination.PageNumber,
            pageSize: pagination.PageSize,
            totalCount: 20,
            items: Enumerable.Range(1, pagination.PageSize!.Value).Select(_ => SearchHistoryFactory.Create()).ToList());
        
        _mockSearchHistoryRepository.Setup(s => s.GetSearchHistory(userId, pagination))
            .ReturnsAsync(expectedHistory);

        // Act
        var actualSearchHistory = await _userService.GetUserSearchHistory(userId, pagination);
        
        // Assert
        Assert.Equal(expectedHistory, actualSearchHistory);
    }
    
    [Fact]
    public async Task GetUserSearchHistoryNotFoundTest()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var pagination = new PaginationParameters(pageNumber: 1, pageSize: 5);
        _mockSearchHistoryRepository.Setup(s => s.GetSearchHistory(userId, pagination))
            .ThrowsAsync(new NotFoundException());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUserSearchHistory(userId, pagination));
    }
    
    [Fact]
    public async Task GetUserSearchHistoryNoPaginationOkTest()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var pagination = new PaginationParameters(pageNumber: null, pageSize: null);
        var expectedHistory = new Paged<SearchHistoryRecord>(pageNumber: 1,
            pageSize: 5,
            totalCount: 5,
            items: Enumerable.Range(1, 5).Select(_ => SearchHistoryFactory.Create()).ToList());
        
        _mockSearchHistoryRepository.Setup(s => s.GetSearchHistory(userId, pagination))
            .ReturnsAsync(expectedHistory);

        // Act
        var actualSearchHistory = await _userService.GetUserSearchHistory(userId, pagination);
        
        // Assert
        Assert.Equal(expectedHistory, actualSearchHistory);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    public async Task GetUserSearchHistoryInvalidPaginationFailTest(int add)
    {
        // Arrange
        var userId = Guid.NewGuid();

        var pageSize = add > 0 ? _pagingConfiguration.MaxPageSize + add : _pagingConfiguration.MinPageSize + add;
        var pagination = new PaginationParameters(pageNumber: 1, pageSize: pageSize);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidPagingException>(() => _userService.GetUserSearchHistory(userId, pagination));
    }
    
    [Fact]
    public async Task GetUserSearchHistoryMaxBorderPaginationOkTest()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var maxPageSize = _pagingConfiguration.MaxPageSize;
        var pagination = new PaginationParameters(pageNumber: 1, pageSize: maxPageSize);
        var expectedHistory = new Paged<SearchHistoryRecord>(pageNumber: pagination.PageNumber,
            pageSize: pagination.PageSize,
            totalCount: 20,
            items: Enumerable.Range(1, pagination.PageSize!.Value).Select(_ => SearchHistoryFactory.Create()).ToList());
        
        _mockSearchHistoryRepository.Setup(s => s.GetSearchHistory(userId, pagination))
            .ReturnsAsync(expectedHistory);

        // Act
        var actualSearchHistory = await _userService.GetUserSearchHistory(userId, pagination);
        
        // Assert
        Assert.Equal(expectedHistory, actualSearchHistory);
    }
    
    [Fact]
    public async Task GetUserSearchHistoryMinBorderPaginationOkTest()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var minPageSize = _pagingConfiguration.MinPageSize;
        var pagination = new PaginationParameters(pageNumber: 1, pageSize: minPageSize);
        var expectedHistory = new Paged<SearchHistoryRecord>(pageNumber: pagination.PageNumber,
            pageSize: pagination.PageSize,
            totalCount: 20,
            items: Enumerable.Range(1, pagination.PageSize!.Value).Select(_ => SearchHistoryFactory.Create()).ToList());
        
        _mockSearchHistoryRepository.Setup(s => s.GetSearchHistory(userId, pagination))
            .ReturnsAsync(expectedHistory);

        // Act
        var actualSearchHistory = await _userService.GetUserSearchHistory(userId, pagination);
        
        // Assert
        Assert.Equal(expectedHistory, actualSearchHistory);
    }
}
using Allure.Xunit.Attributes;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.EntityFrameworkCore;
using Parcorpus.Core.Configuration;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Context;
using Parcorpus.DataAccess.Models;
using Parcorpus.DataAccess.Repositories;
using Parcorpus.Services.UserService;
using Parcorpus.UnitTests.Common.Factories.CoreModels;
using Parcorpus.UnitTests.Common.Factories.DbModels;
using Parcorpus.UnitTests.Common.Helpers;

namespace Parcorpus.UnitTests.Services;

[TestCaseOrderer(
    ordererTypeName: "Parcorpus.UnitTests.Common.Orderers.RandomOrderer",
    ordererAssemblyName: "Parcorpus.UnitTests.Common")]
[AllureSuite("Services")]
[AllureSubSuite("UserServiceClassic")]
public class UserServiceClassicUnitTests
{
    private readonly IUserService _userService;
    
    private readonly IUserRepository _userRepository;
    private readonly ISearchHistoryRepository _searchHistoryRepository;
    
    private readonly Mock<ParcorpusDbContext> _context = new(); // Mocking only shared dependency

    private readonly PagingConfiguration _pagingConfiguration;

    public UserServiceClassicUnitTests()
    {
        var pagingConfiguration = ConfigurationHelper.InitConfiguration<PagingConfiguration>();

        _userRepository = new UserRepository(NullLogger<UserRepository>.Instance, _context.Object);
        _searchHistoryRepository =
            new SearchHistoryRepository(NullLogger<SearchHistoryRepository>.Instance, _context.Object);
        
        _userService = new UserService(NullLogger<UserService>.Instance,
            _userRepository,
            _searchHistoryRepository,
            pagingConfiguration);

        _pagingConfiguration = pagingConfiguration.Value;
    }

    [Fact]
    public async Task GetUserByIdOkTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var countryDb = CountryDbModelFactory.Create();
        var languageDb = LanguageDbModelFactory.Create();
        
        var user = UserFactory.Create(userId: userId, countryName: countryDb.Name, 
            language: new Language(languageDb.ShortName, languageDb.FullName));
        var userDb = UserDbModelFactory.Create(user, countryDb.CountryId, languageDb.LanguageId);
        userDb.CountryNavigation = countryDb;
        userDb.NativeLanguageNavigation = languageDb;

        _context.Setup(s => s.Users)
            .ReturnsDbSet(new List<UserDbModel> { userDb }.AsQueryable());

        // Act
        var actualUser = await _userService.GetUserById(userId);

        // Assert
        Assert.Equal(user, actualUser);
    }
}
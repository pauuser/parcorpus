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

public class UserRepositoryUnitTests
{
    private readonly IUserRepository _userRepository;
    private readonly Mock<ParcorpusDbContext> _context = new();

    public UserRepositoryUnitTests()
    {
        _userRepository = new UserRepository(NullLogger<UserRepository>.Instance, _context.Object);
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
        
        _context.Setup<DbSet<LanguageDbModel>>(s => s.Languages)
            .ReturnsDbSet(new List<LanguageDbModel> { languageDb }.AsQueryable());
        _context.Setup<DbSet<CountryDbModel>>(s => s.Countries)
            .ReturnsDbSet(new List<CountryDbModel> { countryDb }.AsQueryable());
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .ReturnsDbSet(new List<UserDbModel> { userDb }.AsQueryable());
        
        // Act
        var actualUser = await _userRepository.GetUserById(userId);

        // Assert
        Assert.Equal(user, actualUser);
    }
    
    [Fact]
    public async Task GetUserByIdNotFoundTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var countryDb = CountryDbModelFactory.Create();
        var languageDb = LanguageDbModelFactory.Create();
        
        var user = UserFactory.Create(userId: Guid.Empty, countryName: countryDb.Name, 
            language: new Language(languageDb.ShortName, languageDb.FullName));
        var userDb = UserDbModelFactory.Create(user, countryDb.CountryId, languageDb.LanguageId);
        userDb.CountryNavigation = countryDb;
        userDb.NativeLanguageNavigation = languageDb;
        
        _context.Setup<DbSet<LanguageDbModel>>(s => s.Languages)
            .ReturnsDbSet(new List<LanguageDbModel> { languageDb }.AsQueryable());
        _context.Setup<DbSet<CountryDbModel>>(s => s.Countries)
            .ReturnsDbSet(new List<CountryDbModel> { countryDb }.AsQueryable());
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .ReturnsDbSet(new List<UserDbModel> { userDb }.AsQueryable());
        
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userRepository.GetUserById(userId));
    }
    
    [Fact]
    public async Task GetUserByIdExceptionTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var countryDb = CountryDbModelFactory.Create();
        var languageDb = LanguageDbModelFactory.Create();
        
        var user = UserFactory.Create(userId: Guid.Empty, countryName: countryDb.Name, 
            language: new Language(languageDb.ShortName, languageDb.FullName));
        var userDb = UserDbModelFactory.Create(user, countryDb.CountryId, languageDb.LanguageId);
        userDb.CountryNavigation = countryDb;
        userDb.NativeLanguageNavigation = languageDb;
        
        _context.Setup<DbSet<LanguageDbModel>>(s => s.Languages)
            .ReturnsDbSet(new List<LanguageDbModel> { languageDb }.AsQueryable());
        _context.Setup<DbSet<CountryDbModel>>(s => s.Countries)
            .ReturnsDbSet(new List<CountryDbModel> { countryDb }.AsQueryable());
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .Throws<Exception>();
        
        // Act & Assert
        await Assert.ThrowsAsync<UserRepositoryException>(() => _userRepository.GetUserById(userId));
    }
    
    [Fact]
    public async Task RegisterUserByIdOkTest()
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
        
        _context.Setup<DbSet<LanguageDbModel>>(s => s.Languages)
            .ReturnsDbSet(new List<LanguageDbModel> { languageDb }.AsQueryable());
        _context.Setup<DbSet<CountryDbModel>>(s => s.Countries)
            .ReturnsDbSet(new List<CountryDbModel> { countryDb }.AsQueryable());
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .ReturnsDbSet(new List<UserDbModel>().AsQueryable());
        var internalUsers = new List<UserDbModel>();
        _context.Setup(s => s.Users.AddAsync(It.IsAny<UserDbModel>(), It.IsAny<CancellationToken>()))
            .Returns((UserDbModel job, CancellationToken _) =>
            {
                var entry = EntityHelper.GetMockEntityEntry(job);
                entry.Entity.UserId = userId;
                internalUsers.Add(entry.Entity);
                
                return ValueTask.FromResult(entry);
            });
        
        // Act
        var actualUser = await _userRepository.RegisterUser(user);

        // Assert
        Assert.Equal(user, actualUser);
        Assert.NotEmpty(internalUsers);
    }
    
    [Fact]
    public async Task RegisterUserByIdCountryNotFoundTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var countryDb = CountryDbModelFactory.Create();
        var languageDb = LanguageDbModelFactory.Create();
        
        var user = UserFactory.Create(userId: userId, countryName: countryDb.Name, 
            language: new Language(languageDb.ShortName, languageDb.FullName));
        
        _context.Setup<DbSet<LanguageDbModel>>(s => s.Languages)
            .ReturnsDbSet(new List<LanguageDbModel> { languageDb }.AsQueryable());
        _context.Setup<DbSet<CountryDbModel>>(s => s.Countries)
            .ReturnsDbSet(new List<CountryDbModel>().AsQueryable());
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .ReturnsDbSet(new List<UserDbModel>().AsQueryable());
        
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userRepository.RegisterUser(user));
    }
    
    [Fact]
    public async Task RegisterUserByIdLanguageNotFoundTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var countryDb = CountryDbModelFactory.Create();
        var languageDb = LanguageDbModelFactory.Create();
        
        var user = UserFactory.Create(userId: userId, countryName: countryDb.Name, 
            language: new Language(languageDb.ShortName, languageDb.FullName));
        
        _context.Setup<DbSet<LanguageDbModel>>(s => s.Languages)
            .ReturnsDbSet(new List<LanguageDbModel>().AsQueryable());
        _context.Setup<DbSet<CountryDbModel>>(s => s.Countries)
            .ReturnsDbSet(new List<CountryDbModel>() { countryDb }.AsQueryable());
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .ReturnsDbSet(new List<UserDbModel>().AsQueryable());
        
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userRepository.RegisterUser(user));
    }
    
    [Fact]
    public async Task RegisterUserByIdLanguageExceptionTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var countryDb = CountryDbModelFactory.Create();
        var languageDb = LanguageDbModelFactory.Create();
        
        var user = UserFactory.Create(userId: userId, countryName: countryDb.Name, 
            language: new Language(languageDb.ShortName, languageDb.FullName));
        
        _context.Setup<DbSet<LanguageDbModel>>(s => s.Languages)
            .ReturnsDbSet(new List<LanguageDbModel>().AsQueryable());
        _context.Setup<DbSet<CountryDbModel>>(s => s.Countries)
            .Throws<Exception>();
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .ReturnsDbSet(new List<UserDbModel>().AsQueryable());
        
        // Act & Assert
        await Assert.ThrowsAsync<UserRepositoryException>(() => _userRepository.RegisterUser(user));
    }
    
    [Fact]
    public async Task GetUserByEmailOkTest()
    {
        // Arrange
        var email = "inbox@test.com";
        
        var countryDb = CountryDbModelFactory.Create();
        var languageDb = LanguageDbModelFactory.Create();
        
        var user = UserFactory.Create(email: email, countryName: countryDb.Name, 
            language: new Language(languageDb.ShortName, languageDb.FullName));
        var userDb = UserDbModelFactory.Create(user, countryDb.CountryId, languageDb.LanguageId);
        userDb.CountryNavigation = countryDb;
        userDb.NativeLanguageNavigation = languageDb;
        
        _context.Setup<DbSet<LanguageDbModel>>(s => s.Languages)
            .ReturnsDbSet(new List<LanguageDbModel> { languageDb }.AsQueryable());
        _context.Setup<DbSet<CountryDbModel>>(s => s.Countries)
            .ReturnsDbSet(new List<CountryDbModel> { countryDb }.AsQueryable());
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .ReturnsDbSet(new List<UserDbModel> { userDb }.AsQueryable());
        
        // Act
        var actualUser = await _userRepository.GetUserByEmail(email);

        // Assert
        Assert.Equal(user, actualUser);
    }
    
    [Fact]
    public async Task GetUserByEmailNotFoundTest()
    {
        // Arrange
        var email = "inbox@test.com";
        
        var countryDb = CountryDbModelFactory.Create();
        var languageDb = LanguageDbModelFactory.Create();
        
        var user = UserFactory.Create(email: "me@me.ru", countryName: countryDb.Name, 
            language: new Language(languageDb.ShortName, languageDb.FullName));
        var userDb = UserDbModelFactory.Create(user, countryDb.CountryId, languageDb.LanguageId);
        userDb.CountryNavigation = countryDb;
        userDb.NativeLanguageNavigation = languageDb;
        
        _context.Setup<DbSet<LanguageDbModel>>(s => s.Languages)
            .ReturnsDbSet(new List<LanguageDbModel> { languageDb }.AsQueryable());
        _context.Setup<DbSet<CountryDbModel>>(s => s.Countries)
            .ReturnsDbSet(new List<CountryDbModel> { countryDb }.AsQueryable());
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .ReturnsDbSet(new List<UserDbModel> { userDb }.AsQueryable());
        
        // Act
        var actualUser = await _userRepository.GetUserByEmail(email);
        
        // Assert
        Assert.Null(actualUser);
    }
    
    [Fact]
    public async Task GetUserByEmailExceptionTest()
    {
        // Arrange
        var email = "inbox@test.com";
        
        var countryDb = CountryDbModelFactory.Create();
        var languageDb = LanguageDbModelFactory.Create();
        
        var user = UserFactory.Create(email: email, countryName: countryDb.Name, 
            language: new Language(languageDb.ShortName, languageDb.FullName));
        var userDb = UserDbModelFactory.Create(user, countryDb.CountryId, languageDb.LanguageId);
        userDb.CountryNavigation = countryDb;
        userDb.NativeLanguageNavigation = languageDb;
        
        _context.Setup<DbSet<LanguageDbModel>>(s => s.Languages)
            .ReturnsDbSet(new List<LanguageDbModel> { languageDb }.AsQueryable());
        _context.Setup<DbSet<CountryDbModel>>(s => s.Countries)
            .ReturnsDbSet(new List<CountryDbModel> { countryDb }.AsQueryable());
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .Throws<Exception>();
        
        // Act & Assert
        await Assert.ThrowsAsync<UserRepositoryException>(() => _userRepository.GetUserByEmail(email));
    }
    
    [Fact]
    public async Task UpdateUserOkTest()
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
        
        _context.Setup<DbSet<LanguageDbModel>>(s => s.Languages)
            .ReturnsDbSet(new List<LanguageDbModel> { languageDb, LanguageDbModelFactory.Create(3, "fr", "French") }.AsQueryable());
        _context.Setup<DbSet<CountryDbModel>>(s => s.Countries)
            .ReturnsDbSet(new List<CountryDbModel> { countryDb, CountryDbModelFactory.Create(2, "Oceania") }.AsQueryable());
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .ReturnsDbSet(new List<UserDbModel> { userDb }.AsQueryable());
        
        var expectedUpdatedUser = UserFactory.Create(userId: userId, countryName: "Oceania", 
            language: new Language("fr", "French"));
        
        // Act
        var actualUser = await _userRepository.UpdateUser(expectedUpdatedUser);

        // Assert
        Assert.Equal(expectedUpdatedUser, actualUser);
    }
    
    [Fact]
    public async Task UpdateUserCountryNotFoundTest()
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
        
        _context.Setup<DbSet<LanguageDbModel>>(s => s.Languages)
            .ReturnsDbSet(new List<LanguageDbModel> { languageDb, LanguageDbModelFactory.Create(3, "fr", "French") }.AsQueryable());
        _context.Setup<DbSet<CountryDbModel>>(s => s.Countries)
            .ReturnsDbSet(new List<CountryDbModel> { countryDb }.AsQueryable());
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .ReturnsDbSet(new List<UserDbModel> { userDb }.AsQueryable());
        
        var expectedUpdatedUser = UserFactory.Create(userId: userId, countryName: "Oceania", 
            language: new Language(languageDb.ShortName, languageDb.FullName));
        
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userRepository.UpdateUser(expectedUpdatedUser));
    }
    
    [Fact]
    public async Task UpdateUserLanguageNotFoundTest()
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
        
        _context.Setup<DbSet<LanguageDbModel>>(s => s.Languages)
            .ReturnsDbSet(new List<LanguageDbModel> { languageDb }.AsQueryable());
        _context.Setup<DbSet<CountryDbModel>>(s => s.Countries)
            .ReturnsDbSet(new List<CountryDbModel> { countryDb, CountryDbModelFactory.Create(2, "Oceania") }.AsQueryable());
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .ReturnsDbSet(new List<UserDbModel> { userDb }.AsQueryable());
        
        var expectedUpdatedUser = UserFactory.Create(userId: userId, countryName: "Oceania", 
            language: new Language("fr", "French"));
        
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userRepository.UpdateUser(expectedUpdatedUser));
    }
    
    [Fact]
    public async Task UpdateUserExceptionTest()
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

        _context.Setup<DbSet<LanguageDbModel>>(s => s.Languages)
            .Throws<Exception>();
        _context.Setup<DbSet<CountryDbModel>>(s => s.Countries)
            .ReturnsDbSet(new List<CountryDbModel> { countryDb, CountryDbModelFactory.Create(2, "Oceania") }.AsQueryable());
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .ReturnsDbSet(new List<UserDbModel> { userDb }.AsQueryable());
        
        var expectedUpdatedUser = UserFactory.Create(userId: userId, countryName: "Oceania", 
            language: new Language(languageDb.ShortName, languageDb.FullName));
        
        // Act & Assert
        await Assert.ThrowsAsync<UserRepositoryException>(() => _userRepository.UpdateUser(expectedUpdatedUser));
    }
}
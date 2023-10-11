using Allure.Xunit.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.EntityFrameworkCore;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.DataAccess.Context;
using Parcorpus.DataAccess.Models;
using Parcorpus.DataAccess.Repositories;
using Parcorpus.UnitTests.Common.Factories.CoreModels;
using Parcorpus.UnitTests.Common.Factories.DbModels;
using Parcorpus.UnitTests.Common.Helpers;

namespace Parcorpus.UnitTests.DataAccess;

[AllureSuite("Repositories")]
[AllureSubSuite("CredentialsRepository")]
public class CredentialsRepositoryUnitTests
{
    private readonly ICredentialsRepository _credentialsRepository;
    private readonly Mock<ParcorpusDbContext> _context = new();

    public CredentialsRepositoryUnitTests()
    {
        _credentialsRepository = new CredentialsRepository(_context.Object, 
            NullLogger<CredentialsRepository>.Instance);
    }

    [Fact]
    public async Task GetCredentialsOkTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var credentialsDb = CredentialsDbModelFactory.Create(userId: userId);
        var expectedCredentials = CredentialsFactory.Create(credentialsDb);

        _context.Setup<DbSet<CredentialDbModel>>(s => s.Credentials)
            .ReturnsDbSet(new List<CredentialDbModel> { credentialsDb });
        
        // Act
        var actualCredentials = await _credentialsRepository.GetCredentials(userId);

        // Assert
        Assert.Equal(expectedCredentials, actualCredentials);
    }
    
    [Fact]
    public async Task GetCredentialsNotFoundTest()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _context.Setup<DbSet<CredentialDbModel>>(s => s.Credentials)
            .ReturnsDbSet(new List<CredentialDbModel>());
        
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _credentialsRepository.GetCredentials(userId));
    }
    
    [Fact]
    public async Task GetCredentialsExceptionTest()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _context.Setup<DbSet<CredentialDbModel>>(s => s.Credentials)
            .Throws<Exception>();
        
        // Act & Assert
        await Assert.ThrowsAsync<CredentialsRepositoryException>(() => _credentialsRepository.GetCredentials(userId));
    }

    [Fact]
    public async Task CreateCredentialsOkTest()
    {
        // Arrange
        var credential = CredentialsFactory.Create();

        var internalCredentials = new List<CredentialDbModel>();
        _context.Setup(s => s.Credentials.AddAsync(It.IsAny<CredentialDbModel>(), It.IsAny<CancellationToken>()))
            .Returns((CredentialDbModel credentials, CancellationToken _) =>
            {
                var entry = EntityHelper.GetMockEntityEntry(credentials);
                internalCredentials.Add(entry.Entity);
                
                return ValueTask.FromResult(entry);
            });
        
        // Act
        var addedCredentials = await _credentialsRepository.CreateCredentials(credential);

        // Assert
        Assert.NotEmpty(internalCredentials);
        Assert.Equal(credential, addedCredentials);
    }
    
    [Fact]
    public async Task CreateCredentialsExceptionTest()
    {
        // Arrange
        var credential = CredentialsFactory.Create();
        
        _context.Setup(s => s.Credentials.AddAsync(It.IsAny<CredentialDbModel>(), It.IsAny<CancellationToken>()))
            .Throws<Exception>();
        
        // Act & Assert
        await Assert.ThrowsAsync<CredentialsRepositoryException>(() => _credentialsRepository.CreateCredentials(credential));
    }

    [Fact]
    public async Task UpdateRefreshTokenOkTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var credentialsDb = CredentialsDbModelFactory.Create(userId: userId);
        credentialsDb.RefreshToken = "11111";

        var internalList = new List<CredentialDbModel>() { credentialsDb };
        _context.Setup<DbSet<CredentialDbModel>>(s => s.Credentials)
            .ReturnsDbSet(internalList);
        
        var expectedCredentials = CredentialsFactory.Create(credentialsDb);
        expectedCredentials.RefreshToken = "123456";
        expectedCredentials.TokenExpiresAtUtc = DateTime.UtcNow + TimeSpan.FromHours(1);
        
        // Act
        var updatedCredentials = await _credentialsRepository.UpdateRefreshToken(userId, expectedCredentials.RefreshToken,
            expectedCredentials.TokenExpiresAtUtc);

        // Assert
        Assert.Equal(expectedCredentials, updatedCredentials);
    }
    
    [Fact]
    public async Task UpdateRefreshTokenNotFoundTest()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var internalList = new List<CredentialDbModel>();
        _context.Setup<DbSet<CredentialDbModel>>(s => s.Credentials)
            .ReturnsDbSet(internalList);
        
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _credentialsRepository.UpdateRefreshToken(userId, "", new DateTime(2023, 9, 1)));
    }
    
    [Fact]
    public async Task UpdateRefreshTokenExceptionTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        _context.Setup<DbSet<CredentialDbModel>>(s => s.Credentials)
            .Throws<CredentialsRepositoryException>();
        
        // Act & Assert
        await Assert.ThrowsAsync<CredentialsRepositoryException>(() => _credentialsRepository.UpdateRefreshToken(userId, "", new DateTime(2023, 9, 1)));
    }
}
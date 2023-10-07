using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Parcorpus.Core.Configuration;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Services.AuthService;
using Parcorpus.UnitTests.Services.Factories;
using Parcorpus.UnitTests.Services.Helpers;
using HashHelper = Parcorpus.UnitTests.Services.Helpers.HashHelper;

namespace Parcorpus.UnitTests.Services;

public class AuthServiceUnitTests
{
    private readonly IAuthService _authService;

    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<ICredentialsRepository> _mockCredentialsRepository = new();

    private readonly TokenConfiguration _tokenConfiguration;

    public AuthServiceUnitTests()
    {
        var tokenConfigurationOptions = ConfigurationHelper.InitConfiguration<TokenConfiguration>();
        _authService = new AuthService(_mockUserRepository.Object,
            _mockCredentialsRepository.Object,
            tokenConfigurationOptions,
            NullLogger<AuthService>.Instance);

        _tokenConfiguration = tokenConfigurationOptions.Value;
    }

    [Fact]
    public async Task RegisterUserOkTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = UserFactory.Create(userId: userId);

        _mockUserRepository.Setup(s => s.RegisterUser(user))
            .ReturnsAsync(user);
        
        // Act
        var tokens = await _authService.RegisterUser(user);

        // Assert
        var (valid, claimsUserId) = JwtTokenValid(tokens.AccessToken);
        Assert.True(valid);
        Assert.Equal(userId, claimsUserId);
        Assert.NotEmpty(tokens.RefreshToken);
    }

    private (bool, Guid) JwtTokenValid(string jwt)
    {
        var valid = true;
        var userId = Guid.Empty;
        
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfiguration.Key)),
            ValidateLifetime = true,
            ValidAudience = _tokenConfiguration.Audience,
            ValidIssuer = _tokenConfiguration.Issuer
        };

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(jwt, tokenValidationParameters, out var securityToken);

            if (securityToken is JwtSecurityToken jwtSecurityToken &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                userId = Guid.Parse(principal.Claims.First(c => c.Type == "userId").Value);
            }
            else
            {
                valid = false;
            }
        }
        catch (Exception)
        {
            valid = false;
        }

        return (valid, userId);
    }
    
    [Fact]
    public async Task RegisterUserExistsFailTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = UserFactory.Create(userId: userId);

        _mockUserRepository.Setup(s => s.GetUserByEmail(user.Email.Address))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(s => s.RegisterUser(user))
            .ReturnsAsync(user);
        
        // Act & Assert
        await Assert.ThrowsAsync<UserExistsException>(() => _authService.RegisterUser(user));
    }
    
    [Fact]
    public async Task LoginUserOkTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string password = "test_password";
        var passwordHash = HashHelper.Sha256Hash(password);
        var refreshToken = "123123";
        
        var user = UserFactory.Create(userId: userId, passwordHash: passwordHash);
        var credentials = CredentialsFactory.Create(userId: userId, 
            refreshToken: refreshToken);
        
        _mockUserRepository.Setup(s => s.GetUserByEmail(user.Email.Address))
            .ReturnsAsync(user);
        _mockCredentialsRepository.Setup(s => s.GetCredentials(userId))
            .ReturnsAsync(credentials);
        
        // Act
        var tokens = await _authService.LoginUser(user.Email.Address, password);

        // Assert
        var (valid, claimsUserId) = JwtTokenValid(tokens.AccessToken);
        Assert.True(valid);
        Assert.Equal(userId, claimsUserId);
        Assert.Equal(refreshToken, tokens.RefreshToken);
    }
    
    [Fact]
    public async Task LoginUserExpiredRefreshTokenOkTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string password = "test_password";
        var passwordHash = HashHelper.Sha256Hash(password);
        var refreshToken = "123123";
        
        var user = UserFactory.Create(userId: userId, passwordHash: passwordHash);
        var credentials = CredentialsFactory.Create(userId: userId, 
            refreshToken: refreshToken,
            tokenExpiresAtUtc: DateTime.UtcNow - TimeSpan.FromDays(1));
        
        _mockUserRepository.Setup(s => s.GetUserByEmail(user.Email.Address))
            .ReturnsAsync(user);
        _mockCredentialsRepository.Setup(s => s.GetCredentials(userId))
            .ReturnsAsync(credentials);
        
        // Act
        var tokens = await _authService.LoginUser(user.Email.Address, password);

        // Assert
        var (valid, claimsUserId) = JwtTokenValid(tokens.AccessToken);
        Assert.True(valid);
        Assert.Equal(userId, claimsUserId);
        Assert.NotEqual(refreshToken, tokens.RefreshToken);
    }
    
    [Fact]
    public async Task LoginUserNotFoundTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string password = "test_password";
        var passwordHash = HashHelper.Sha256Hash(password);
        var refreshToken = "123123";
        
        var user = UserFactory.Create(userId: userId, passwordHash: passwordHash);
        var credentials = CredentialsFactory.Create(userId: userId, 
            refreshToken: refreshToken);
        
        _mockCredentialsRepository.Setup(s => s.GetCredentials(userId))
            .ReturnsAsync(credentials);
        
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _authService.LoginUser(user.Email.Address, password));
    }
    
    [Fact]
    public async Task LoginUserInvalidPasswordTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string password = "test_password";
        var passwordHash = HashHelper.Sha256Hash(password);
        var refreshToken = "123123";
        
        var user = UserFactory.Create(userId: userId, passwordHash: passwordHash);
        var credentials = CredentialsFactory.Create(userId: userId, 
            refreshToken: refreshToken);
        
        _mockUserRepository.Setup(s => s.GetUserByEmail(user.Email.Address))
            .ReturnsAsync(user);
        _mockCredentialsRepository.Setup(s => s.GetCredentials(userId))
            .ReturnsAsync(credentials);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidPasswordException>(() => _authService.LoginUser(user.Email.Address, "1111"));
    }

    [Fact]
    public async Task RefreshTokenOkTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var jwt = JwtHelper.CreateJwt(userId, _tokenConfiguration, DateTime.UtcNow);
        
        const string refreshToken = "123123";
        var credentials = CredentialsFactory.Create(userId: userId, 
            refreshToken: refreshToken);

        _mockCredentialsRepository.Setup(s => s.GetCredentials(userId))
            .ReturnsAsync(credentials);
        
        // Act
        var tokens = await _authService.RefreshToken(jwt, refreshToken);

        // Assert
        var (valid, claimsUserId) = JwtTokenValid(tokens.AccessToken);
        Assert.True(valid);
        Assert.Equal(userId, claimsUserId);
        Assert.NotEmpty(tokens.RefreshToken);
        Assert.NotEqual(refreshToken, tokens.RefreshToken);
        Assert.NotEqual(jwt, tokens.AccessToken);
    }
    
    [Fact]
    public async Task RefreshTokenParseFailTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        const string refreshToken = "123123";
        var credentials = CredentialsFactory.Create(userId: userId, 
            refreshToken: refreshToken);

        _mockCredentialsRepository.Setup(s => s.GetCredentials(userId))
            .ReturnsAsync(credentials);
        
        // Act & Assert
        await Assert.ThrowsAsync<SecurityTokenException>(() => _authService.RefreshToken("999", refreshToken));
    }
    
    [Fact]
    public async Task RefreshTokenInvalidSignatureTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var configuration = TokenConfigurationFactory.Create();
        var jwt = JwtHelper.CreateJwt(userId, configuration, DateTime.UtcNow);
        
        const string refreshToken = "123123";
        var credentials = CredentialsFactory.Create(userId: userId, 
            refreshToken: refreshToken);

        _mockCredentialsRepository.Setup(s => s.GetCredentials(userId))
            .ReturnsAsync(credentials);
        
        // Act & Assert
        await Assert.ThrowsAsync<SecurityTokenException>(() => _authService.RefreshToken(jwt, refreshToken));
    }
    
    [Fact]
    public async Task RefreshTokenExpiredRefreshTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var jwt = JwtHelper.CreateJwt(userId, _tokenConfiguration, DateTime.UtcNow);
        
        const string refreshToken = "123123";
        var credentials = CredentialsFactory.Create(userId: userId, 
            refreshToken: refreshToken, tokenExpiresAtUtc: DateTime.UtcNow - TimeSpan.FromHours(1));

        _mockCredentialsRepository.Setup(s => s.GetCredentials(userId))
            .ReturnsAsync(credentials);
        
        // Act & Assert
        await Assert.ThrowsAsync<ExpiredRefreshTokenException>(() => _authService.RefreshToken(jwt, refreshToken));
    }
    
    [Fact]
    public async Task RefreshTokenInvalidRefreshTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var jwt = JwtHelper.CreateJwt(userId, _tokenConfiguration, DateTime.UtcNow);
        
        const string refreshToken = "123123";
        var credentials = CredentialsFactory.Create(userId: userId, 
            refreshToken: refreshToken);

        _mockCredentialsRepository.Setup(s => s.GetCredentials(userId))
            .ReturnsAsync(credentials);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidRefreshTokenException>(() => _authService.RefreshToken(jwt, "000"));
    }
}
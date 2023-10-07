using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
}
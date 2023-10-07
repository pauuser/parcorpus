using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Parcorpus.Core.Configuration;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;
using Parcorpus.Services.Helpers;

namespace Parcorpus.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ICredentialsRepository _credentialsRepository;
    private readonly ILogger<AuthService> _logger;
    private readonly TokenConfiguration _jwtConfiguration;

    public AuthService(IUserRepository userRepository, 
        ICredentialsRepository credentialsRepository, 
        IOptions<TokenConfiguration> configuration, 
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _credentialsRepository =
            credentialsRepository ?? throw new ArgumentNullException(nameof(credentialsRepository));
        _jwtConfiguration = configuration.Value ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TokenPair> RegisterUser(User newUser)
    {
        var existingUser = await _userRepository.GetUserByEmail(newUser.Email.Address);
        if (existingUser is not null)
        {
            _logger.LogError("Cannot register user that already exists with email {mail}", existingUser.Email.Address);
            throw new UserExistsException($"Cannot register user that already exists with email {existingUser.Email.Address}");
        }
        
        var user = await _userRepository.RegisterUser(newUser);

        var jwt = await GetJwtToken(user.UserId);
        var refreshToken = GenerateRefreshToken();

        await _credentialsRepository.CreateCredentials(new Credential(credentialId: default,
            userId: user.UserId,
            refreshToken: refreshToken,
            tokenExpiresAtUtc: DateTime.UtcNow + _jwtConfiguration.RefreshTokenExpiresIn));

        return new TokenPair(jwt, refreshToken);
    }

    public async Task<TokenPair> LoginUser(string email, string password)
    {
        var user = await _userRepository.GetUserByEmail(email);
        if (user is null)
        {
            _logger.LogError("User with email = {email} does not exist", email);
            throw new NotFoundException($"User with email = {email} does not exist");
        }

        var passwordHash = HashHelper.Sha256Hash(password);
        if (passwordHash != user.PasswordHash)
        {
            _logger.LogError("Invalid password for user {userId}", user.UserId);
            throw new InvalidPasswordException($"Invalid password for user {user.UserId}");
        }
        
        var jwt = await GetJwtToken(user.UserId);
        var credentials = await _credentialsRepository.GetCredentials(user.UserId);
        var refreshToken = credentials.RefreshToken;
        if (DateTime.UtcNow > credentials.TokenExpiresAtUtc)
        {
            _logger.LogError("Refresh token for user {userId} expired", user.UserId);
            refreshToken = GenerateRefreshToken();

            await _credentialsRepository.UpdateRefreshToken(user.UserId, refreshToken,
                DateTime.UtcNow + _jwtConfiguration.RefreshTokenExpiresIn);
        }

        return new TokenPair(jwt, refreshToken);
    }

    public async Task<TokenPair> RefreshToken(string jwt, string refreshToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key)),
            ValidateLifetime = false,
            ValidAudience = _jwtConfiguration.Audience,
            ValidIssuer = _jwtConfiguration.Issuer
        };

        SecurityToken securityToken;
        ClaimsPrincipal principal;
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            principal = tokenHandler.ValidateToken(jwt, tokenValidationParameters, out securityToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token parsing failed");
            throw new SecurityTokenException("Token parsing failed", ex);
        }
        
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            _logger.LogError("Got invalid token");
            throw new SecurityTokenException("Token is invalid");
        }
        
        var userId = Guid.Parse(principal.Claims.First(c => c.Type == "userId").Value);

        var credentials = await _credentialsRepository.GetCredentials(userId);
        if (DateTime.UtcNow > credentials.TokenExpiresAtUtc)
        {
            _logger.LogError("Cannot refresh token for user {userId} because refresh token is expired", userId);
            throw new ExpiredRefreshTokenException($"Cannot refresh token for user {userId} because refresh token is expired");
        }

        if (credentials.RefreshToken != refreshToken)
        {
            _logger.LogError("Invalid refresh token was given for userId = {userId}", userId);
            throw new InvalidRefreshTokenException("Invalid refresh token was given");
        }
        
        var newRefreshToken = GenerateRefreshToken();
        var newJwt = await GetJwtToken(userId);
        await _credentialsRepository.UpdateRefreshToken(userId, newRefreshToken,
            DateTime.UtcNow + _jwtConfiguration.RefreshTokenExpiresIn);

        return new TokenPair(newJwt, newRefreshToken);
    }

    private Task<string> GetJwtToken(Guid userId)
    {
        var claims = new List<Claim>
        {
            new ("userId", userId.ToString())
        };
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var jwt = new JwtSecurityToken(issuer: _jwtConfiguration.Issuer,
            audience: _jwtConfiguration.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(_jwtConfiguration.ExpiresIn),
            signingCredentials: credentials
        );

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(jwt));
    }
    
    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);
        
        return Convert.ToBase64String(randomNumber);
    }
}
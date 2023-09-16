using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Parcorpus.API.Converters;
using Parcorpus.API.Dto;
using Parcorpus.Core.Configuration;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;

namespace Parcorpus.API.Controllers;

/// <summary>
/// Controller for user registration and login
/// </summary>
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;
    private readonly LanguagesConfiguration _languagesConfiguration;
    
    /// <summary>
    /// Auth controller constructor
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="authService">Authentication service</param>
    /// <param name="languageConfiguration">Languages configuration</param>
    public AuthController(ILogger<AuthController> logger, 
        IAuthService authService, 
        IOptions<LanguagesConfiguration> languageConfiguration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _languagesConfiguration = languageConfiguration.Value ?? throw new ArgumentNullException(nameof(languageConfiguration));
    }
    
    /// <summary>
    /// Create new account
    /// </summary>
    /// <param name="registrationDto">Registration information</param>
    /// <returns>Tokens</returns>
    /// <response code="200">OK. Tokens returned.</response>
    /// <response code="409">Conflict. User already exists.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokensDto))]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
    {
        try
        {
            var tokens = await _authService.RegisterUser(UserConverter.ConvertDtoToAppModel(registrationDto,
                new Language(registrationDto.LanguageShortName, _languagesConfiguration.LanguagesForms[registrationDto.LanguageShortName]), 
                registrationDto.Password));
            
            return Ok(TokenConverter.ConvertAppModelToDto(tokens));
        }
        catch (UserExistsException ex)
        {
            _logger.LogError(ex, "Conflict: {message}", ex.Message);
            return Conflict($"Conflict: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Internal server error: {message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Login into account
    /// </summary>
    /// <param name="loginDto">Login information</param>
    /// <returns>Tokens</returns>
    /// <response code="200">OK. Tokens returned.</response>
    /// <response code="401">Unauthorized. Invalid password.</response>
    /// <response code="404">Not found. User with such email doesn't exist.</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokensDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var tokens = await _authService.LoginUser(loginDto.Email, loginDto.Password);

            return Ok(TokenConverter.ConvertAppModelToDto(tokens));
        }
        catch (InvalidPasswordException ex)
        {
            _logger.LogError(ex, "Unauthorized: {message}", ex.Message);
            return Unauthorized($"Unauthorized: {ex.Message}");
        }
        catch (NotFoundException ex)
        {
            _logger.LogError(ex, "Not found: {message}", ex.Message);
            return NotFound($"Not found: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Internal server error: {message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Refresh token
    /// </summary>
    /// <param name="tokens">Access token and refresh token</param>
    /// <returns>Tokens</returns>
    /// <response code="200">OK. Tokens returned.</response>
    /// <response code="401">Unauthorized. Token invalid or expired.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokensDto))]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Refresh([FromBody] TokensDto tokens)
    {
        try
        {
            var updatedTokens = await _authService.RefreshToken(tokens.AccessToken, tokens.RefreshToken);

            return Ok(TokenConverter.ConvertAppModelToDto(updatedTokens));
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogError(ex, "Unauthorized: {message}", ex.Message);
            return Unauthorized($"Unauthorized: {ex.Message}");
        }
        catch (ExpiredRefreshTokenException ex)
        {
            _logger.LogError(ex, "Unauthorized: {message}", ex.Message);
            return Unauthorized($"Unauthorized: {ex.Message}");
        }
        catch (InvalidRefreshTokenException ex)
        {
            _logger.LogError(ex, "Unauthorized: {message}", ex.Message);
            return Unauthorized($"Unauthorized: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Internal server error: {message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Parcorpus.API.Converters;
using Parcorpus.API.Dto;
using Parcorpus.API.Extensions;
using Parcorpus.Core.Configuration;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;

namespace Parcorpus.API.Controllers;

/// <summary>
/// Controller for user management
/// </summary>
[ApiController]
[Route("api/v1/users")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    
    private readonly LanguagesConfiguration _languagesConfiguration;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="userService">User service</param>
    /// <exception cref="ArgumentNullException"></exception>
    public UserController(ILogger<UserController> logger, IUserService userService, IOptions<LanguagesConfiguration> languagesConfiguration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _languagesConfiguration =
            languagesConfiguration.Value ?? throw new ArgumentNullException(nameof(languagesConfiguration));
    }
    
    /// <summary>
    /// Get user information via token
    /// </summary>
    /// <returns>Tokens</returns>
    /// <response code="200">OK. User returned.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Internal server error.</response>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Me()
    {
        try
        {
            var userId = HttpContext.Request.GetUserId();
            var user = await _userService.GetUserById(userId);
            
            return Ok(UserConverter.ConvertAppModelToDto(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Internal server error: {message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Update user information
    /// </summary>
    /// <returns>Tokens</returns>
    /// <response code="200">OK. User returned.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Internal server error.</response>
    [Authorize]
    [HttpPatch("me")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PatchMe([FromBody] JsonPatchDocument<UserDto> patchDoc)
    {
        try
        {
            var userId = HttpContext.Request.GetUserId();
            var user = await _userService.GetUserById(userId);

            var toBePatched = UserConverter.ConvertAppModelToDto(user);
            patchDoc.ApplyTo(toBePatched);
            var updated = await _userService.UpdateUser(user,
                UserConverter.ConvertDtoToAppModel(user: toBePatched,
                    userNativeLanguage: new Language(toBePatched.NativeLanguageShortName,
                        _languagesConfiguration.LanguagesForms[toBePatched.NativeLanguageShortName]),
                    passwordHash: user.PasswordHash));

            return Ok(UserConverter.ConvertAppModelToDto(updated));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, "Bad Request. Invalid language: {message}", ex.Message);
            return BadRequest($"Bad Request. Invalid language: {ex.Message}");
        }
        catch (ImpossiblePatchException ex)
        {
            _logger.LogError(ex, "Bad request: {message}", ex.Message);
            return BadRequest($"Bad request: {ex.Message}");
        }
        catch (NotFoundException ex)
        {
            _logger.LogError(ex, "Bad request: {message}", ex.Message);
            return BadRequest($"Bad request: {ex.Message}");
        }
        catch (FormatException ex)
        {
            _logger.LogError(ex, "Invalid format: {message}", ex.Message);
            return BadRequest($"Invalid format: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Internal server error: {message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Get user's search history
    /// </summary>
    /// <param name="page">Number of the page</param>
    /// <param name="pageSize">Size of the page</param>
    /// <returns>Tokens</returns>
    /// <response code="200">OK. Search history returned.</response>
    /// <response code="400">Bad Request. Invalid paging.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not found. Search history is empty.</response>
    /// <response code="500">Internal server error.</response>
    [Authorize]
    [HttpGet("history")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedDto<SearchHistoryDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> History([FromQuery] int? page = null, 
        [FromQuery(Name = "page_size")] int? pageSize = null)
    {
        try
        {
            var userId = HttpContext.Request.GetUserId();
            var history = await _userService.GetUserSearchHistory(userId: userId,
                paging: new PaginationParameters(page, pageSize));

            return Ok(PagedConverter.ConvertAppModelToDto(history));
        }
        catch (InvalidPagingException ex)
        {
            _logger.LogError(ex, "Bad Request: invalid paging. See: {message}", ex.Message);
            return BadRequest("Bad Request: Invalid paging.");
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
    /// Get user by Id
    /// </summary>
    /// <returns>Tokens</returns>
    /// <response code="200">OK. Tokens returned.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error.</response>
    [Authorize]
    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UserInfo([FromRoute] Guid userId)
    {
        try
        {
            var user = await _userService.GetUserById(userId);
            
            return Ok(UserConverter.ConvertAppModelToDto(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Internal server error: {message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
}
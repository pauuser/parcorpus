using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Parcorpus.API.Converters;
using Parcorpus.API.Dto;
using Parcorpus.API.Extensions;
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

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="userService">User service</param>
    /// <exception cref="ArgumentNullException"></exception>
    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }
    
    /// <summary>
    /// Get user information via token
    /// </summary>
    /// <returns>Tokens</returns>
    /// <response code="200">OK. User returned.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("me")]
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
    /// Get user's search history
    /// </summary>
    /// <returns>Tokens</returns>
    /// <response code="200">OK. Search history returned.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not found. Search history is empty.</response>
    /// <response code="500">Internal server error.</response>
    [Authorize]
    [HttpGet("history")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SearchHistoryRecord>))]
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
            
            return Ok(history.Select(SearchHistoryConverter.ConvertAppModelToDto));
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
/// Controller for managing corpus actions
/// </summary>
[ApiController]
[Route("api/v1/texts")]
public class TextsController : ControllerBase
{
    private readonly ILogger<TextsController> _logger;
    private readonly ILanguageService _languageService;
    private readonly LanguagesConfiguration _languagesConfiguration;

    /// <summary>
    /// Corpus controller constructor
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="languageService">Language service</param>
    /// <param name="languagesConfiguration">Language names dictionary</param>
    public TextsController(ILogger<TextsController> logger, 
        ILanguageService languageService, 
        IOptions<LanguagesConfiguration> languagesConfiguration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _languageService = languageService ?? throw new ArgumentNullException(nameof(languageService));
        _languagesConfiguration = languagesConfiguration.Value ?? throw new ArgumentNullException(nameof(languagesConfiguration));
    }

    /// <summary>
    /// Get word usage examples (word's concordance)
    /// </summary>
    /// <param name="query">Filters for the search</param>
    /// <param name="page">Number of the page</param>
    /// <param name="pageSize">Size of the page</param>
    /// <returns>List of word usage examples</returns>
    /// <response code="200">OK. Concordance returned.</response>
    /// <response code="400">Bad Request. Invalid input or invalid paging.</response>
    /// <response code="401">Unauthorized. User with the given id does not exist or he has no rights.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Internal server error.</response>
    [Authorize]
    [HttpGet("concordance")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedDto<ConcordanceDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetConcordance([FromQuery]ConcordanceQueryDto query, 
        [FromQuery] int? page = null, 
        [FromQuery(Name = "page_size")] int? pageSize = null)
    {
        try
        {
            var userId = HttpContext.Request.GetUserId();
            if (!_languagesConfiguration.LanguagesForms.ContainsKey(query.SourceLanguageShortName) ||
                !_languagesConfiguration.LanguagesForms.ContainsKey(query.TargetLanguageShortName))
            {
                _logger.LogError("One of the languages is null: source = {source}, target = {target}",
                    query.SourceLanguageShortName, query.TargetLanguageShortName);
                throw new ArgumentException(
                    $"One of the languages is null: source = {query.SourceLanguageShortName}, " +
                    $"target = {query.TargetLanguageShortName}");
            }

            var result = await _languageService.GetConcordance(userId: userId, 
                query: new ConcordanceQuery(
                    sourceWord: new Word(query.Word, new Language(query.SourceLanguageShortName,
                        _languagesConfiguration.LanguagesForms[query.SourceLanguageShortName])),
                    destinationLanguage: new Language(query.TargetLanguageShortName,
                        _languagesConfiguration.LanguagesForms[query.TargetLanguageShortName]),
                    filters: FilterConverter.ConvertDtoToAppModel(query.Filter)),
                paging: new PaginationParameters(page, pageSize));

            return Ok(PagedConverter.ConvertAppModelToDto(result));
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "BadRequest: {message}", ex.Message);
            return BadRequest($"BadRequest: {ex.Message}");
        }
        catch (NoTokenException ex)
        {
            _logger.LogError(ex, "Unauthorized: {message}", ex.Message);
            return Unauthorized($"Unauthorized: {ex.Message}");
        }
        catch (NotFoundException ex)
        {
            _logger.LogError(ex, "Not found: {message}", ex.Message);
            return NotFound($"Not found: {ex.Message}");
        }
        catch (InvalidPagingException ex)
        {
            _logger.LogError(ex, "Bad Request: paging is invalid. See: {message}", ex.Message);
            return BadRequest("Bad Request: Paging is invalid.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Internal server error: {message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Get texts uploaded by the user
    /// </summary>
    /// <param name="page">Number of the page</param>
    /// <param name="pageSize">Size of the page</param>
    /// <returns>List of text records</returns>
    /// <response code="200">OK. Texts returned.</response>
    /// <response code="400">Bad request. Invalid paging.</response>
    /// <response code="401">Unauthorized. User with the given id does not exist or he has no rights.</response>
    /// <response code="404">Not found. Information for the given user is not found.</response>
    /// <response code="500">Internal server error.</response>
    [Authorize]
    [HttpGet("texts")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedDto<TextDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserTexts([FromQuery] int? page = null, 
        [FromQuery(Name = "page_size")] int? pageSize = null)
    {
        try
        {
            var userId = HttpContext.Request.GetUserId();
            var result = await _languageService.GetTextsAddedByUser(userId: userId,
                paging: new PaginationParameters(page, pageSize));
            
            return Ok(PagedConverter.ConvertAppModelToDto(result));
        }
        catch (NoTokenException ex)
        {
            _logger.LogError("Unauthorized: {message}", ex.Message);
            return Unauthorized($"Unauthorized: {ex.Message}");
        }
        catch (InvalidPagingException ex)
        {
            _logger.LogError(ex, "Bad Request: paging is invalid. See: {message}", ex.Message);
            return BadRequest("Bad Request: Paging is invalid.");
        }
        catch (NotFoundException ex)
        {
            _logger.LogError("Not found: {message}", ex.Message);
            return NotFound($"Not found: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError("Internal server error: {message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Get text by Id
    /// </summary>
    /// <returns>List of text records</returns>
    /// <remarks>Paging is specified for sentences.</remarks>
    /// <response code="200">OK. Texts returned.</response>
    /// <response code="400">Bad request. Invalid paging.</response>
    /// <response code="401">Unauthorized. User with the given id does not exist or he has no rights.</response>
    /// <response code="404">Not found. Information for the given user is not found.</response>
    /// <response code="500">Internal server error.</response>
    [Authorize]
    [HttpGet("texts/{textId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedTextDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTextById([FromRoute] int textId,
        [FromQuery] int? page = null, 
        [FromQuery(Name = "page_size")] int? pageSize = null) 
    {
        try
        {
            var result = await _languageService.GetTextById(textId: textId,
                paging: new PaginationParameters(page, pageSize));
            
            return Ok(PagedConverter.ConvertAppModelToDto(result));
        }
        catch (NoTokenException ex)
        {
            _logger.LogError("Unauthorized: {message}", ex.Message);
            return Unauthorized($"Unauthorized: {ex.Message}");
        }
        catch (InvalidPagingException ex)
        {
            _logger.LogError(ex, "Bad Request: paging is invalid. See: {message}", ex.Message);
            return BadRequest("Bad Request: Paging is invalid.");
        }
        catch (NotFoundException ex)
        {
            _logger.LogError("Not found: {message}", ex.Message);
            return NotFound($"Not found: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError("Internal server error: {message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Delete text for the textId
    /// </summary>
    /// <param name="textId">Text id of the text to be deleted</param>
    /// <returns></returns>
    /// <response code="200">OK. Text deleted.</response>
    /// <response code="401">Unauthorized. User with the given id does not exist or he has no rights.</response>
    /// <response code="404">Not found. Text with such id for the given user is not found.</response>
    /// <response code="500">Internal server error.</response>
    [Authorize]
    [HttpDelete("texts/{textId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteText([FromRoute] int textId)
    {
        try
        {
            var userId = HttpContext.Request.GetUserId();
            await _languageService.DeleteText(userId, textId);
            
            return Ok();
        }
        catch (NoTokenException ex)
        {
            _logger.LogError("Unauthorized: {message}", ex.Message);
            return Unauthorized($"Unauthorized: {ex.Message}");
        }
        catch (NotFoundException ex)
        {
            _logger.LogError("Text was not found for the user: {message}", ex.Message);
            return NotFound($"Text was not found for the user: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError("Internal server error: {message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
}
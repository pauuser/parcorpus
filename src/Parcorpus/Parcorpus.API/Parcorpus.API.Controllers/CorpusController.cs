using System.Net.Http.Headers;
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
using Parcorpus.Services.Helpers;

namespace Parcorpus.API.Controllers;

/// <summary>
/// Controller for managing corpus actions
/// </summary>
[ApiController]
[Route("api/v1/corpus")]
public class CorpusController : ControllerBase
{
    private readonly ILogger<CorpusController> _logger;
    private readonly ILanguageService _languageService;
    private readonly LanguagesConfiguration _languagesConfiguration;

    /// <summary>
    /// Corpus controller constructor
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="languageService">Language service</param>
    /// <param name="languagesConfiguration">Language names dictionary</param>
    public CorpusController(ILogger<CorpusController> logger, 
        ILanguageService languageService, 
        IOptions<LanguagesConfiguration> languagesConfiguration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _languageService = languageService ?? throw new ArgumentNullException(nameof(languageService));
        _languagesConfiguration = languagesConfiguration.Value ?? throw new ArgumentNullException(nameof(languagesConfiguration));
    }

    /// <summary>
    /// Upload text
    /// </summary>
    /// <param name="textInfo">Text information</param>
    /// <returns>Nothing</returns>
    /// <response code="201">Created. Text was added.</response>
    /// <response code="400">Bad Request. Language or text is invalid.</response>
    /// <response code="401">Unauthorized. Such user id doesn't belong to any user.</response>
    /// <response code="409">Conflict. Text already exists.</response>
    /// <response code="500">Internal server error.</response>
    [Authorize]
    [HttpPost("upload")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadText([FromForm] InputTextDto textInfo)
    {
        try
        {
            var userId = HttpContext.Request.GetUserId();
            if (!_languagesConfiguration.LanguagesForms.ContainsKey(textInfo.SourceLanguageCode) ||
                !_languagesConfiguration.LanguagesForms.ContainsKey(textInfo.TargetLanguageCode))
            {
                _logger.LogError("One of the languages is null: source = {source}, target = {target}",
                    textInfo.SourceLanguageCode, textInfo.TargetLanguageCode);
                throw new ArgumentException($"One of the languages is null: source = {textInfo.SourceLanguageCode}, " +
                    $"target = {textInfo.TargetLanguageCode}");
            }

            var sourceText = FormStringReader.ReadFormFileToString(textInfo.SourceText);
            var targetText = FormStringReader.ReadFormFileToString(textInfo.TargetText);

            await _languageService.UploadText(userId,
                new BiText(sourceText: sourceText,
                    targetText: targetText,
                    sourceLanguage: new Language(shortName: textInfo.SourceLanguageCode,
                        fullEnglishName: _languagesConfiguration.LanguagesForms[textInfo.SourceLanguageCode]),
                    targetLanguage: new Language(shortName: textInfo.TargetLanguageCode,
                        fullEnglishName: _languagesConfiguration.LanguagesForms[textInfo.TargetLanguageCode]),
                    new MetaAnnotation(textInfo.Title,
                        textInfo.Author,
                        textInfo.Source,
                        textInfo.CreationYear,
                        DateTime.UtcNow),
                    textInfo.Genres.ToList()));

            return StatusCode(StatusCodes.Status201Created);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Bad Request: {message}", ex.Message);
            return BadRequest($"Bad Request: {ex.Message}");
        }
        catch (NoTokenException ex)
        {
            _logger.LogError(ex, "Unauthorized: {message}", ex.Message);
            return Unauthorized($"Unauthorized: {ex.Message}");
        }
        catch (TextAlreadyExistsException ex)
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
    /// Get word usage examples (word's concordance)
    /// </summary>
    /// <param name="query">Filters for the search</param>
    /// <returns>List of word usage examples</returns>
    /// <response code="200">OK. Concordance returned.</response>
    /// <response code="400">Bad Request. Invalid input.</response>
    /// <response code="401">Unauthorized. User with the given id does not exist or he has no rights.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Internal server error.</response>
    [Authorize]
    [HttpGet("concordance")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ConcordanceDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetConcordance([FromQuery]ConcordanceQueryDto query)
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

            var result = await _languageService.GetConcordance(userId, new ConcordanceQuery(
                sourceWord: new Word(query.Word, new Language(query.SourceLanguageShortName,
                    _languagesConfiguration.LanguagesForms[query.SourceLanguageShortName])),
                destinationLanguage: new Language(query.TargetLanguageShortName,
                    _languagesConfiguration.LanguagesForms[query.TargetLanguageShortName]),
                filters: FilterConverter.ConvertDtoToAppModel(query.Filter)));

            return Ok(result.Select(ConcordanceConverter.ConvertAppModelToDto));
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
        catch (Exception ex)
        {
            _logger.LogError("Internal server error: {message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Get texts uploaded by the user
    /// </summary>
    /// <returns>List of text records</returns>
    /// <response code="200">OK. Texts returned.</response>
    /// <response code="401">Unauthorized. User with the given id does not exist or he has no rights.</response>
    /// <response code="404">Not found. Information for the given user is not found.</response>
    /// <response code="500">Internal server error.</response>
    [Authorize]
    [HttpGet("texts")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TextDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserTexts()
    {
        try
        {
            var userId = HttpContext.Request.GetUserId();
            var result = await _languageService.GetTextsAddedByUser(userId);
            
            return Ok(result.Select(TextConverter.ConvertAppModelToDto));
        }
        catch (NoTokenException ex)
        {
            _logger.LogError("Unauthorized: {message}", ex.Message);
            return Unauthorized($"Unauthorized: {ex.Message}");
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
    /// <response code="200">OK. Texts returned.</response>
    /// <response code="401">Unauthorized. User with the given id does not exist or he has no rights.</response>
    /// <response code="404">Not found. Information for the given user is not found.</response>
    /// <response code="500">Internal server error.</response>
    [Authorize]
    [HttpGet("texts/{textId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FullTextDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTextById([FromRoute] int textId) 
    {
        try
        {
            var result = await _languageService.GetTextById(textId);
            
            return Ok(TextConverter.ConvertFullTextToDto(result));
        }
        catch (NoTokenException ex)
        {
            _logger.LogError("Unauthorized: {message}", ex.Message);
            return Unauthorized($"Unauthorized: {ex.Message}");
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
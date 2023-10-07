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
/// Controller for jobs management
/// </summary>
[ApiController]
[Route("api/v1/jobs")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;
    private readonly ILogger<JobsController> _logger;
    private readonly LanguagesConfiguration _languagesConfiguration;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="jobService">Job service</param>
    /// <param name="logger">logger</param>
    /// <param name="configuration"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public JobsController(IJobService jobService, 
        ILogger<JobsController> logger,
        IOptions<LanguagesConfiguration> configuration)
    {
        _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _languagesConfiguration = configuration.Value ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Upload text
    /// </summary>
    /// <param name="textInfo">Text information</param>
    /// <returns>Nothing</returns>
    /// <response code="200">Ok. Job was created.</response>
    /// <response code="400">Bad Request. Language or text is invalid.</response>
    /// <response code="401">Unauthorized. Such user id doesn't belong to any user.</response>
    /// <response code="409">Conflict. Text already exists.</response>
    /// <response code="500">Internal server error.</response>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JobDto))]
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

            var job = await _jobService.UploadText(userId,
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

            return Ok(JobConverter.ConvertAppModelToDto(job));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, "Bad Request. Invalid language: {message}", ex.Message);
            return BadRequest($"Bad Request. Invalid language: {ex.Message}");
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
    /// Get user's latest upload jobs
    /// </summary>
    /// <returns>Nothing</returns>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad request. Invalid paging.</response>
    /// <response code="401">Unauthorized. Such user id doesn't belong to any user.</response>
    /// <response code="404">Not found. User has no jobs.</response>
    /// <response code="500">Internal server error.</response>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Paged<JobDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LatestJobs([FromQuery] int? page = null, 
        [FromQuery(Name = "page_size")] int? pageSize = null)
    {
        try
        {
            var userId = HttpContext.Request.GetUserId();
            var jobs = await _jobService.GetUserJobs(userId: userId,
                paging: new PaginationParameters(page, pageSize));

            return Ok(PagedConverter.ConvertAppModelToDto(jobs));
        }
        catch (NoTokenException ex)
        {
            _logger.LogError(ex, "Unauthorized: {message}", ex.Message);
            return Unauthorized($"Unauthorized: {ex.Message}");
        }
        catch (InvalidPagingException ex)
        {
            _logger.LogError(ex, "Bad Request: paging is invalid. See: {message}", ex.Message);
            return BadRequest("Bad Request: Paging is invalid.");
        }
        catch (JobNotFoundException ex)
        {
            _logger.LogError(ex, "Not found: {message}", ex.Message);
            return Conflict($"Not found: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Internal server error: {message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Get job by Id
    /// </summary>
    /// <returns>Nothing</returns>
    /// <response code="200">Ok. Text was added.</response>
    /// <response code="401">Unauthorized. Such user id doesn't belong to any user.</response>
    /// <response code="404">Not found. Job with such id doesn't exist.</response>
    /// <response code="500">Internal server error.</response>
    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JobDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetJobById([FromRoute] Guid id)
    {
        try
        {
            var job = await _jobService.GetJobById(id);
            
            return Ok(JobConverter.ConvertAppModelToDto(job));
        }
        catch (NoTokenException ex)
        {
            _logger.LogError(ex, "Unauthorized: {message}", ex.Message);
            return Unauthorized($"Unauthorized: {ex.Message}");
        }
        catch (JobNotFoundException ex)
        {
            _logger.LogError(ex, "Not found: {message}", ex.Message);
            return Conflict($"Not found: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Internal server error: {message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
}
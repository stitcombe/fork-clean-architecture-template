using BoricuaCoder.CleanTemplate.Application.Common.CQRS;
using BoricuaCoder.CleanTemplate.Application.Common.Results;
using BoricuaCoder.CleanTemplate.Application.Movies.Commands.CreateMovie;
using BoricuaCoder.CleanTemplate.Application.Movies.Commands.ImportMoviesFromOmdb;
using BoricuaCoder.CleanTemplate.Application.Movies.DTOs;
using BoricuaCoder.CleanTemplate.Application.Movies.Queries.GetMovieById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoricuaCoder.CleanTemplate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MoviesController : ControllerBase
{
    private readonly ICommandHandler<CreateMovieCommand, Result<MovieResponse>> _createHandler;
    private readonly IQueryHandler<GetMovieByIdQuery, Result<MovieResponse>> _getByIdHandler;
    private readonly ICommandHandler<ImportMoviesFromOmdbCommand, Result<IReadOnlyList<MovieResponse>>> _importHandler;

    public MoviesController(
        ICommandHandler<CreateMovieCommand, Result<MovieResponse>> createHandler,
        IQueryHandler<GetMovieByIdQuery, Result<MovieResponse>> getByIdHandler,
        ICommandHandler<ImportMoviesFromOmdbCommand, Result<IReadOnlyList<MovieResponse>>> importHandler)
    {
        _createHandler = createHandler;
        _getByIdHandler = getByIdHandler;
        _importHandler = importHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateMovieCommand command, CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(command, cancellationToken);
        return ToActionResult(result, () => CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getByIdHandler.HandleAsync(new GetMovieByIdQuery(id), cancellationToken);
        return ToActionResult(result, () => Ok(result.Value));
    }

    [HttpPost("import")]
    [ProducesResponseType(typeof(IReadOnlyList<MovieResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ImportFromOmdb(
        [FromBody] ImportMoviesFromOmdbCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _importHandler.HandleAsync(command, cancellationToken);
        return ToActionResult(result, () => Ok(result.Value));
    }

    private IActionResult ToActionResult<T>(Result<T> result, Func<IActionResult> onSuccess)
    {
        return result.Type switch
        {
            ResultType.Success => onSuccess(),
            ResultType.NotFound => NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = result.Error
            }),
            ResultType.Conflict => Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = result.Error
            }),
            ResultType.ValidationError => BadRequest(new ValidationProblemDetails(
                result.ValidationErrors!.ToDictionary(kvp => kvp.Key, kvp => kvp.Value))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error"
            }),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}

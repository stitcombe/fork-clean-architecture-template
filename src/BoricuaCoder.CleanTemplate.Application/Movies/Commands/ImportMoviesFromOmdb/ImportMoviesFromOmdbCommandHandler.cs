using BoricuaCoder.CleanTemplate.Application.Common.CQRS;
using BoricuaCoder.CleanTemplate.Application.Common.Results;
using BoricuaCoder.CleanTemplate.Application.Movies.DTOs;
using BoricuaCoder.CleanTemplate.Domain.Movies;
using FluentValidation;

namespace BoricuaCoder.CleanTemplate.Application.Movies.Commands.ImportMoviesFromOmdb;

public class ImportMoviesFromOmdbCommandHandler
    : ICommandHandler<ImportMoviesFromOmdbCommand, Result<IReadOnlyList<MovieResponse>>>
{
    private readonly IOmdbService _omdbService;
    private readonly IMovieRepository _movieRepository;
    private readonly IValidator<ImportMoviesFromOmdbCommand> _validator;

    public ImportMoviesFromOmdbCommandHandler(
        IOmdbService omdbService,
        IMovieRepository movieRepository,
        IValidator<ImportMoviesFromOmdbCommand> validator)
    {
        _omdbService = omdbService;
        _movieRepository = movieRepository;
        _validator = validator;
    }

    public async Task<Result<IReadOnlyList<MovieResponse>>> HandleAsync(
        ImportMoviesFromOmdbCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return Result<IReadOnlyList<MovieResponse>>.ValidationError(errors);
        }

        var omdbResults = await _omdbService.SearchMoviesAsync(command.SearchTerm, cancellationToken);

        if (omdbResults.Count == 0)
            return Result<IReadOnlyList<MovieResponse>>.NotFound(
                $"No movies found on OMDB for '{command.SearchTerm}'.");

        var imported = new List<MovieResponse>();

        foreach (var omdbMovie in omdbResults)
        {
            if (!int.TryParse(omdbMovie.Year.Split('–')[0].Trim(), out var year))
                continue;

            var exists = await _movieRepository.ExistsByTitleAndYearAsync(omdbMovie.Title, year, cancellationToken);
            if (exists)
                continue;

            var movie = Movie.Create(omdbMovie.Title, year, omdbMovie.ImdbId);
            await _movieRepository.CreateAsync(movie, cancellationToken);

            imported.Add(new MovieResponse(movie.Id, movie.Title, movie.Year, movie.ImdbId));
        }

        return Result<IReadOnlyList<MovieResponse>>.Success(imported);
    }
}

using BoricuaCoder.CleanTemplate.Application.Common.CQRS;
using BoricuaCoder.CleanTemplate.Application.Common.Results;
using BoricuaCoder.CleanTemplate.Application.Movies.DTOs;
using BoricuaCoder.CleanTemplate.Domain.Movies;
using FluentValidation;

namespace BoricuaCoder.CleanTemplate.Application.Movies.Commands.CreateMovie;

public class CreateMovieCommandHandler : ICommandHandler<CreateMovieCommand, Result<MovieResponse>>
{
    private readonly IMovieRepository _movieRepository;
    private readonly IValidator<CreateMovieCommand> _validator;

    public CreateMovieCommandHandler(IMovieRepository movieRepository, IValidator<CreateMovieCommand> validator)
    {
        _movieRepository = movieRepository;
        _validator = validator;
    }

    public async Task<Result<MovieResponse>> HandleAsync(CreateMovieCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return Result<MovieResponse>.ValidationError(errors);
        }

        var exists = await _movieRepository.ExistsByTitleAndYearAsync(command.Title, command.Year, cancellationToken);
        if (exists)
            return Result<MovieResponse>.Conflict($"A movie with title '{command.Title}' and year {command.Year} already exists.");

        var movie = Movie.Create(command.Title, command.Year, command.ImdbId);
        await _movieRepository.CreateAsync(movie, cancellationToken);

        var response = new MovieResponse(movie.Id, movie.Title, movie.Year, movie.ImdbId);
        return Result<MovieResponse>.Success(response);
    }
}

using BoricuaCoder.CleanTemplate.Application.Common.CQRS;
using BoricuaCoder.CleanTemplate.Application.Common.Results;
using BoricuaCoder.CleanTemplate.Application.Movies.DTOs;

namespace BoricuaCoder.CleanTemplate.Application.Movies.Queries.GetMovieById;

public class GetMovieByIdQueryHandler : IQueryHandler<GetMovieByIdQuery, Result<MovieResponse>>
{
    private readonly IMovieRepository _movieRepository;

    public GetMovieByIdQueryHandler(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<Result<MovieResponse>> HandleAsync(GetMovieByIdQuery query, CancellationToken cancellationToken = default)
    {
        var movie = await _movieRepository.GetByIdAsync(query.Id, cancellationToken);

        if (movie is null)
            return Result<MovieResponse>.NotFound($"Movie with id '{query.Id}' was not found.");

        var response = new MovieResponse(movie.Id, movie.Title, movie.Year, movie.ImdbId);
        return Result<MovieResponse>.Success(response);
    }
}

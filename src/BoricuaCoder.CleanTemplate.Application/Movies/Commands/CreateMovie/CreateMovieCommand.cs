using BoricuaCoder.CleanTemplate.Application.Common.CQRS;
using BoricuaCoder.CleanTemplate.Application.Common.Results;
using BoricuaCoder.CleanTemplate.Application.Movies.DTOs;

namespace BoricuaCoder.CleanTemplate.Application.Movies.Commands.CreateMovie;

public record CreateMovieCommand(string Title, int Year, string? ImdbId) : ICommand<Result<MovieResponse>>;

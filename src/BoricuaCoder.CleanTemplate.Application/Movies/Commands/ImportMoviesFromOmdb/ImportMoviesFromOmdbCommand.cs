using BoricuaCoder.CleanTemplate.Application.Common.CQRS;
using BoricuaCoder.CleanTemplate.Application.Common.Results;
using BoricuaCoder.CleanTemplate.Application.Movies.DTOs;

namespace BoricuaCoder.CleanTemplate.Application.Movies.Commands.ImportMoviesFromOmdb;

public record ImportMoviesFromOmdbCommand(string SearchTerm) : ICommand<Result<IReadOnlyList<MovieResponse>>>;

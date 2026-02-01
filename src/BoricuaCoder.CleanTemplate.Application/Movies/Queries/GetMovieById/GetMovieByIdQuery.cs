using BoricuaCoder.CleanTemplate.Application.Common.CQRS;
using BoricuaCoder.CleanTemplate.Application.Common.Results;
using BoricuaCoder.CleanTemplate.Application.Movies.DTOs;

namespace BoricuaCoder.CleanTemplate.Application.Movies.Queries.GetMovieById;

public record GetMovieByIdQuery(Guid Id) : IQuery<Result<MovieResponse>>;

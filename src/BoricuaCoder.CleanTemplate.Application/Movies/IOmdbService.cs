using BoricuaCoder.CleanTemplate.Application.Movies.DTOs;

namespace BoricuaCoder.CleanTemplate.Application.Movies;

public interface IOmdbService
{
    Task<IReadOnlyList<OmdbMovieResult>> SearchMoviesAsync(string searchTerm, CancellationToken cancellationToken = default);
}

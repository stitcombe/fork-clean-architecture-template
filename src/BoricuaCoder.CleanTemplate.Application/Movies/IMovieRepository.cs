using BoricuaCoder.CleanTemplate.Domain.Movies;

namespace BoricuaCoder.CleanTemplate.Application.Movies;

public interface IMovieRepository
{
    Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task CreateAsync(Movie movie, CancellationToken cancellationToken = default);
    Task<bool> ExistsByTitleAndYearAsync(string title, int year, CancellationToken cancellationToken = default);
}

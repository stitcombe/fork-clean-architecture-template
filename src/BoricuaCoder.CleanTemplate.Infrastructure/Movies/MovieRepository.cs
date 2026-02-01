using BoricuaCoder.CleanTemplate.Application.Common.Interfaces;
using BoricuaCoder.CleanTemplate.Application.Movies;
using BoricuaCoder.CleanTemplate.Domain.Movies;
using Dapper;

namespace BoricuaCoder.CleanTemplate.Infrastructure.Movies;

public class MovieRepository : IMovieRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public MovieRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Movie>(
            "SELECT id AS Id, title AS Title, year AS Year, imdb_id AS ImdbId FROM movies WHERE id = @Id",
            new { Id = id });
    }

    public async Task CreateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        await connection.ExecuteAsync(
            "INSERT INTO movies (id, title, year, imdb_id) VALUES (@Id, @Title, @Year, @ImdbId)",
            new { movie.Id, movie.Title, movie.Year, movie.ImdbId });
    }

    public async Task<bool> ExistsByTitleAndYearAsync(string title, int year, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        return await connection.ExecuteScalarAsync<bool>(
            "SELECT EXISTS(SELECT 1 FROM movies WHERE title = @Title AND year = @Year)",
            new { Title = title, Year = year });
    }
}

using BoricuaCoder.CleanTemplate.Domain.Exceptions;

namespace BoricuaCoder.CleanTemplate.Domain.Movies;

public class Movie
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = default!;
    public int Year { get; private set; }
    public string? ImdbId { get; private set; }

    private Movie() { }

    public static Movie Create(string title, int year, string? imdbId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Movie title is required.");

        if (year < 1888 || year > DateTime.UtcNow.Year + 5)
            throw new DomainException($"Movie year must be between 1888 and {DateTime.UtcNow.Year + 5}.");

        return new Movie
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Year = year,
            ImdbId = string.IsNullOrWhiteSpace(imdbId) ? null : imdbId.Trim()
        };
    }
}

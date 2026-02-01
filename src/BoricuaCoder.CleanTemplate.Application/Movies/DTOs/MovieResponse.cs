namespace BoricuaCoder.CleanTemplate.Application.Movies.DTOs;

public record MovieResponse(Guid Id, string Title, int Year, string? ImdbId);

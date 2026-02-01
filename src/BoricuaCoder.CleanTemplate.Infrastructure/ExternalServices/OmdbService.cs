using System.Net.Http.Json;
using System.Text.Json.Serialization;
using BoricuaCoder.CleanTemplate.Application.Movies;
using BoricuaCoder.CleanTemplate.Application.Movies.DTOs;

namespace BoricuaCoder.CleanTemplate.Infrastructure.ExternalServices;

public class OmdbService : IOmdbService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OmdbService(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<IReadOnlyList<OmdbMovieResult>> SearchMoviesAsync(
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<OmdbSearchResponse>(
            $"?apikey={_apiKey}&s={Uri.EscapeDataString(searchTerm)}&type=movie",
            cancellationToken);

        if (response is null || response.Response != "True" || response.Search is null)
            return [];

        return response.Search
            .Select(r => new OmdbMovieResult(r.Title, r.Year, r.ImdbID))
            .ToList();
    }

    private sealed class OmdbSearchResponse
    {
        [JsonPropertyName("Search")]
        public List<OmdbSearchItem>? Search { get; set; }

        [JsonPropertyName("totalResults")]
        public string? TotalResults { get; set; }

        [JsonPropertyName("Response")]
        public string Response { get; set; } = default!;
    }

    private sealed class OmdbSearchItem
    {
        [JsonPropertyName("Title")]
        public string Title { get; set; } = default!;

        [JsonPropertyName("Year")]
        public string Year { get; set; } = default!;

        [JsonPropertyName("imdbID")]
        public string ImdbID { get; set; } = default!;

        [JsonPropertyName("Type")]
        public string Type { get; set; } = default!;

        [JsonPropertyName("Poster")]
        public string Poster { get; set; } = default!;
    }
}

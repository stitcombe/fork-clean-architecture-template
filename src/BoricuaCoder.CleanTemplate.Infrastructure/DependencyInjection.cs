using BoricuaCoder.CleanTemplate.Application.Common.Interfaces;
using BoricuaCoder.CleanTemplate.Application.Movies;
using BoricuaCoder.CleanTemplate.Infrastructure.Database;
using BoricuaCoder.CleanTemplate.Infrastructure.ExternalServices;
using BoricuaCoder.CleanTemplate.Infrastructure.Movies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BoricuaCoder.CleanTemplate.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(connectionString));
        services.AddScoped<IMovieRepository, MovieRepository>();

        var omdbApiKey = configuration["Omdb:ApiKey"]
            ?? throw new InvalidOperationException("OMDB API key 'Omdb:ApiKey' is not configured.");

        services.AddHttpClient("Omdb", client =>
        {
            client.BaseAddress = new Uri("https://www.omdbapi.com/");
        });

        services.AddScoped<IOmdbService>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var client = factory.CreateClient("Omdb");
            return new OmdbService(client, omdbApiKey);
        });

        return services;
    }
}

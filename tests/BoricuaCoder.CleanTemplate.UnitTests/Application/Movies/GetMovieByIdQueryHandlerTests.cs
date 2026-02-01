using BoricuaCoder.CleanTemplate.Application.Common.Results;
using BoricuaCoder.CleanTemplate.Application.Movies;
using BoricuaCoder.CleanTemplate.Application.Movies.Queries.GetMovieById;
using BoricuaCoder.CleanTemplate.Domain.Movies;
using Moq;

namespace BoricuaCoder.CleanTemplate.UnitTests.Application.Movies;

[TestFixture]
public class GetMovieByIdQueryHandlerTests
{
    private Mock<IMovieRepository> _movieRepositoryMock = null!;
    private GetMovieByIdQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _movieRepositoryMock = new Mock<IMovieRepository>();
        _handler = new GetMovieByIdQueryHandler(_movieRepositoryMock.Object);
    }

    [Test]
    public async Task HandleAsync_WithExistingMovie_ReturnsSuccess()
    {
        var movie = Movie.Create("Inception", 2010, "tt1375666");
        _movieRepositoryMock
            .Setup(r => r.GetByIdAsync(movie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);

        var result = await _handler.HandleAsync(new GetMovieByIdQuery(movie.Id));

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value!.Title, Is.EqualTo("Inception"));
        Assert.That(result.Value.Year, Is.EqualTo(2010));
        Assert.That(result.Value.ImdbId, Is.EqualTo("tt1375666"));
    }

    [Test]
    public async Task HandleAsync_WithNonExistingMovie_ReturnsNotFound()
    {
        var movieId = Guid.NewGuid();
        _movieRepositoryMock
            .Setup(r => r.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie?)null);

        var result = await _handler.HandleAsync(new GetMovieByIdQuery(movieId));

        Assert.That(result.Type, Is.EqualTo(ResultType.NotFound));
        Assert.That(result.Error, Does.Contain("not found"));
    }
}

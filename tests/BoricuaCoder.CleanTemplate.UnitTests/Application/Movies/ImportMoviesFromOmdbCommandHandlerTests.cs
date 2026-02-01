using BoricuaCoder.CleanTemplate.Application.Common.Results;
using BoricuaCoder.CleanTemplate.Application.Movies;
using BoricuaCoder.CleanTemplate.Application.Movies.Commands.ImportMoviesFromOmdb;
using BoricuaCoder.CleanTemplate.Application.Movies.DTOs;
using BoricuaCoder.CleanTemplate.Domain.Movies;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace BoricuaCoder.CleanTemplate.UnitTests.Application.Movies;

[TestFixture]
public class ImportMoviesFromOmdbCommandHandlerTests
{
    private Mock<IOmdbService> _omdbServiceMock = null!;
    private Mock<IMovieRepository> _movieRepositoryMock = null!;
    private Mock<IValidator<ImportMoviesFromOmdbCommand>> _validatorMock = null!;
    private ImportMoviesFromOmdbCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _omdbServiceMock = new Mock<IOmdbService>();
        _movieRepositoryMock = new Mock<IMovieRepository>();
        _validatorMock = new Mock<IValidator<ImportMoviesFromOmdbCommand>>();
        _handler = new ImportMoviesFromOmdbCommandHandler(
            _omdbServiceMock.Object,
            _movieRepositoryMock.Object,
            _validatorMock.Object);

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ImportMoviesFromOmdbCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    [Test]
    public async Task HandleAsync_WithResults_ImportsNewMovies()
    {
        var command = new ImportMoviesFromOmdbCommand("Inception");
        _omdbServiceMock
            .Setup(s => s.SearchMoviesAsync("Inception", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OmdbMovieResult>
            {
                new("Inception", "2010", "tt1375666"),
                new("Inception: The Cobol Job", "2010", "tt1790736")
            });
        _movieRepositoryMock
            .Setup(r => r.ExistsByTitleAndYearAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.HandleAsync(command);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Has.Count.EqualTo(2));
        Assert.That(result.Value![0].Title, Is.EqualTo("Inception"));
        Assert.That(result.Value[1].Title, Is.EqualTo("Inception: The Cobol Job"));
        _movieRepositoryMock.Verify(
            r => r.CreateAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Test]
    public async Task HandleAsync_SkipsDuplicates()
    {
        var command = new ImportMoviesFromOmdbCommand("Inception");
        _omdbServiceMock
            .Setup(s => s.SearchMoviesAsync("Inception", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OmdbMovieResult>
            {
                new("Inception", "2010", "tt1375666"),
                new("Inception: The Cobol Job", "2010", "tt1790736")
            });
        _movieRepositoryMock
            .Setup(r => r.ExistsByTitleAndYearAsync("Inception", 2010, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _movieRepositoryMock
            .Setup(r => r.ExistsByTitleAndYearAsync("Inception: The Cobol Job", 2010, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.HandleAsync(command);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Has.Count.EqualTo(1));
        Assert.That(result.Value![0].Title, Is.EqualTo("Inception: The Cobol Job"));
        _movieRepositoryMock.Verify(
            r => r.CreateAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task HandleAsync_WithNoResults_ReturnsNotFound()
    {
        var command = new ImportMoviesFromOmdbCommand("xyznonexistent");
        _omdbServiceMock
            .Setup(s => s.SearchMoviesAsync("xyznonexistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OmdbMovieResult>());

        var result = await _handler.HandleAsync(command);

        Assert.That(result.Type, Is.EqualTo(ResultType.NotFound));
        Assert.That(result.Error, Does.Contain("No movies found"));
    }

    [Test]
    public async Task HandleAsync_WithValidationError_ReturnsValidationError()
    {
        var command = new ImportMoviesFromOmdbCommand("");
        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("SearchTerm", "Search term is required.")
            }));

        var result = await _handler.HandleAsync(command);

        Assert.That(result.Type, Is.EqualTo(ResultType.ValidationError));
        Assert.That(result.ValidationErrors, Does.ContainKey("SearchTerm"));
        _omdbServiceMock.Verify(
            s => s.SearchMoviesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task HandleAsync_SkipsMoviesWithUnparsableYear()
    {
        var command = new ImportMoviesFromOmdbCommand("test");
        _omdbServiceMock
            .Setup(s => s.SearchMoviesAsync("test", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OmdbMovieResult>
            {
                new("Good Movie", "2020", "tt1111111"),
                new("Bad Year Movie", "N/A", "tt2222222")
            });
        _movieRepositoryMock
            .Setup(r => r.ExistsByTitleAndYearAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.HandleAsync(command);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Has.Count.EqualTo(1));
        Assert.That(result.Value![0].Title, Is.EqualTo("Good Movie"));
    }
}

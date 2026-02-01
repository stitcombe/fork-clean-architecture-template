using BoricuaCoder.CleanTemplate.Application.Common.Results;
using BoricuaCoder.CleanTemplate.Application.Movies;
using BoricuaCoder.CleanTemplate.Application.Movies.Commands.CreateMovie;
using BoricuaCoder.CleanTemplate.Application.Movies.DTOs;
using BoricuaCoder.CleanTemplate.Domain.Movies;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace BoricuaCoder.CleanTemplate.UnitTests.Application.Movies;

[TestFixture]
public class CreateMovieCommandHandlerTests
{
    private Mock<IMovieRepository> _movieRepositoryMock = null!;
    private Mock<IValidator<CreateMovieCommand>> _validatorMock = null!;
    private CreateMovieCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _movieRepositoryMock = new Mock<IMovieRepository>();
        _validatorMock = new Mock<IValidator<CreateMovieCommand>>();
        _handler = new CreateMovieCommandHandler(_movieRepositoryMock.Object, _validatorMock.Object);

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateMovieCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    [Test]
    public async Task HandleAsync_WithValidCommand_ReturnsSuccess()
    {
        var command = new CreateMovieCommand("Inception", 2010, "tt1375666");
        _movieRepositoryMock
            .Setup(r => r.ExistsByTitleAndYearAsync("Inception", 2010, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.HandleAsync(command);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value!.Title, Is.EqualTo("Inception"));
        Assert.That(result.Value.Year, Is.EqualTo(2010));
        Assert.That(result.Value.ImdbId, Is.EqualTo("tt1375666"));
        _movieRepositoryMock.Verify(
            r => r.CreateAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task HandleAsync_WithDuplicateMovie_ReturnsConflict()
    {
        var command = new CreateMovieCommand("Inception", 2010, null);
        _movieRepositoryMock
            .Setup(r => r.ExistsByTitleAndYearAsync("Inception", 2010, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.HandleAsync(command);

        Assert.That(result.Type, Is.EqualTo(ResultType.Conflict));
        Assert.That(result.Error, Does.Contain("already exists"));
        _movieRepositoryMock.Verify(
            r => r.CreateAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task HandleAsync_WithValidationErrors_ReturnsValidationError()
    {
        var command = new CreateMovieCommand("", 2010, null);
        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("Title", "Title is required.")
            }));

        var result = await _handler.HandleAsync(command);

        Assert.That(result.Type, Is.EqualTo(ResultType.ValidationError));
        Assert.That(result.ValidationErrors, Does.ContainKey("Title"));
        _movieRepositoryMock.Verify(
            r => r.CreateAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}

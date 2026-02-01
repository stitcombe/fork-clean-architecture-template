using BoricuaCoder.CleanTemplate.Domain.Exceptions;
using BoricuaCoder.CleanTemplate.Domain.Movies;

namespace BoricuaCoder.CleanTemplate.UnitTests.Domain;

[TestFixture]
public class MovieTests
{
    [Test]
    public void Create_WithValidData_ReturnsMovie()
    {
        var movie = Movie.Create("Inception", 2010, "tt1375666");

        Assert.That(movie, Is.Not.Null);
        Assert.That(movie.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(movie.Title, Is.EqualTo("Inception"));
        Assert.That(movie.Year, Is.EqualTo(2010));
        Assert.That(movie.ImdbId, Is.EqualTo("tt1375666"));
    }

    [Test]
    public void Create_WithoutImdbId_SetsImdbIdToNull()
    {
        var movie = Movie.Create("Inception", 2010);

        Assert.That(movie.ImdbId, Is.Null);
    }

    [Test]
    public void Create_WithEmptyTitle_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => Movie.Create("", 2010));
        Assert.That(ex!.Message, Does.Contain("title"));
    }

    [Test]
    public void Create_WithWhitespaceTitle_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => Movie.Create("   ", 2010));
        Assert.That(ex!.Message, Does.Contain("title"));
    }

    [Test]
    public void Create_WithYearTooLow_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => Movie.Create("Test", 1800));
        Assert.That(ex!.Message, Does.Contain("year"));
    }

    [Test]
    public void Create_WithYearTooHigh_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => Movie.Create("Test", DateTime.UtcNow.Year + 10));
        Assert.That(ex!.Message, Does.Contain("year"));
    }

    [Test]
    public void Create_TrimsTitle()
    {
        var movie = Movie.Create("  Inception  ", 2010);

        Assert.That(movie.Title, Is.EqualTo("Inception"));
    }

    [Test]
    public void Create_WithWhitespaceImdbId_SetsImdbIdToNull()
    {
        var movie = Movie.Create("Inception", 2010, "   ");

        Assert.That(movie.ImdbId, Is.Null);
    }
}

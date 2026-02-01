using FluentValidation;

namespace BoricuaCoder.CleanTemplate.Application.Movies.Commands.ImportMoviesFromOmdb;

public class ImportMoviesFromOmdbCommandValidator : AbstractValidator<ImportMoviesFromOmdbCommand>
{
    public ImportMoviesFromOmdbCommandValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotEmpty().WithMessage("Search term is required.")
            .MinimumLength(2).WithMessage("Search term must be at least 2 characters.");
    }
}

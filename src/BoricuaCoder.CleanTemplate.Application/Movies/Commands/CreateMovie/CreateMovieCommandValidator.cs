using FluentValidation;

namespace BoricuaCoder.CleanTemplate.Application.Movies.Commands.CreateMovie;

public class CreateMovieCommandValidator : AbstractValidator<CreateMovieCommand>
{
    public CreateMovieCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Year)
            .InclusiveBetween(1888, DateTime.UtcNow.Year + 5)
            .WithMessage($"Year must be between 1888 and {DateTime.UtcNow.Year + 5}.");

        RuleFor(x => x.ImdbId)
            .Matches(@"^tt\d+$")
            .When(x => !string.IsNullOrWhiteSpace(x.ImdbId))
            .WithMessage("ImdbId must be in format 'tt' followed by digits (e.g. tt1234567).");
    }
}

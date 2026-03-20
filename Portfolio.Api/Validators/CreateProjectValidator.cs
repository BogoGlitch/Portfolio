using FluentValidation;
using Portfolio.Api.Dtos.Projects;

namespace Portfolio.Api.Validators;

public class CreateProjectValidator : AbstractValidator<CreateProjectDto>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(150)
            .Matches("^[a-z0-9-]+$")
            .WithMessage("Slug must contain only lowercase letters, numbers, and hyphens.");

        RuleFor(x => x.ShortDescription)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.FullDescription)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.RepoUrl)
            .MaximumLength(500)
            .Must(BeAValidUrl)
            .WithMessage("RepoUrl must be a valid URL.")
            .When(x => !string.IsNullOrEmpty(x.RepoUrl));

        RuleFor(x => x.LiveUrl)
            .MaximumLength(500)
            .Must(BeAValidUrl)
            .WithMessage("LiveUrl must be a valid URL.")
            .When(x => !string.IsNullOrEmpty(x.LiveUrl));

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500)
            .Must(BeAValidUrl)
            .WithMessage("ImageUrl must be a valid URL.")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }

    private static bool BeAValidUrl(string? url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var result)
        && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
}

using FluentValidation;
using Portfolio.Api.Dtos.Technologies;

namespace Portfolio.Api.Validators;

public class CreateTechnologyValidator : AbstractValidator<CreateTechnologyDto>
{
    public CreateTechnologyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(100)
            .Matches("^[a-z0-9-]+$")
            .WithMessage("Slug must contain only lowercase letters, numbers, and hyphens.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Category)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.LogoUrl)
            .MaximumLength(500)
            .Must(BeAValidUrl)
            .WithMessage("LogoUrl must be a valid URL.")
            .When(x => !string.IsNullOrEmpty(x.LogoUrl));

        RuleFor(x => x.DocumentationUrl)
            .MaximumLength(500)
            .Must(BeAValidUrl)
            .WithMessage("DocumentationUrl must be a valid URL.")
            .When(x => !string.IsNullOrEmpty(x.DocumentationUrl));

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }

    private static bool BeAValidUrl(string? url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var result)
        && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
}

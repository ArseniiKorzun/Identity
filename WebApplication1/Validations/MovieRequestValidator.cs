using FluentValidation;
using WebApplication1.Contracts;

namespace WebApplication1.Validations
{
    public abstract class MovieRequestValidator<T> : AbstractValidator<T> where T : IMovieRequest
    {
        protected MovieRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.PhotoPath)
                .MaximumLength(500).WithMessage("Photo path cannot exceed 500 characters.");

            RuleFor(x => x.Director)
                .NotEmpty().WithMessage("Director is required.")
                .MaximumLength(100).WithMessage("Director cannot exceed 100 characters.");

            RuleFor(x => x.Runtime)
                .InclusiveBetween(1, 600).WithMessage("Runtime must be between 1 and 600 minutes.");

            RuleFor(x => x.ReleaseYear)
                .GreaterThanOrEqualTo(1888).WithMessage("Release year cannot be earlier than 1888.")
                .Must(year => year <= DateTime.UtcNow.Year + 1)
                .WithMessage("Release year cannot be more than one year in the future.");

            RuleFor(x => x.PosterUrl)
                .MaximumLength(500).WithMessage("Poster URL cannot exceed 500 characters.")
                .Must(IsHttpUrl).WithMessage("Poster URL must be a valid http or https link.")
                .When(x => !string.IsNullOrEmpty(x.PosterUrl));

            RuleFor(x => x.MovieUrl)
                .MaximumLength(500).WithMessage("Movie URL cannot exceed 500 characters.")
                .Must(IsHttpUrl).WithMessage("Movie URL must be a valid http or https link.")
                .When(x => !string.IsNullOrEmpty(x.MovieUrl));

            RuleFor(x => x.Genre)
                .MaximumLength(50).WithMessage("Genre cannot exceed 50 characters.");

            RuleFor(x => x.Country)
                .MaximumLength(50).WithMessage("Country cannot exceed 50 characters.");
        }

        private static bool IsHttpUrl(string url) =>
            Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}

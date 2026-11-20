using FluentValidation;
using WebApplication1.Contracts;

namespace WebApplication1.Validations
{
    public class RegisterRequestValidator : AbstractValidator<IdentityRegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty().WithMessage("Login is required.")
                .MinimumLength(5).WithMessage("Login must be at least 5 characters long.")
                .MaximumLength(50).WithMessage("Login cannot exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.")
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Invalid email format.")
                .EmailAddress().WithMessage("Invalid email address.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required.")
                .Equal(x => x.Password).WithMessage("Passwords do not match.");

            RuleFor(x => x.DisplayName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Display name is required.")
                .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Display name cannot contain only spaces.")
                .Must(name => name.Trim().Length >= 3 && name.Trim().Length <= 30)
                .WithMessage("Display name must be from 3 to 30 characters.");

            RuleFor(x => x.City)
                .MaximumLength(60).WithMessage("City cannot exceed 60 characters.");

            RuleFor(x => x.FavoriteGenre)
                .MaximumLength(50).WithMessage("Favorite genre cannot exceed 50 characters.");
        }
    }
}

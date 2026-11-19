using FluentValidation;
using WebApplication1.Contracts;

namespace WebApplication1.Validations
{
    public class IdentityLoginRequestValidator : AbstractValidator<IdentityLoginRequest>
    {
        public IdentityLoginRequestValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty().WithMessage("Login is required.")
                .MinimumLength(5).WithMessage("Login must be at least 5 characters long.")
                .MaximumLength(50).WithMessage("Login cannot exceed 50 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.");
        }
    }
}

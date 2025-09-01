using FluentValidation;

namespace MerchantApp.API.Validators
{
    /// Ensures that email and password meet the required rules.
    public class MerchantLoginDTOValidator : AbstractValidator<DTOs.MerchantLoginRequestDTO>
    {
        public MerchantLoginDTOValidator()
        {
            // Email must not be empty and must be in a valid format
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email cannot be empty.")
                .EmailAddress().WithMessage("Invalid email format.");

            // Password must not be empty and must have at least 6 characters
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password cannot be empty.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }
    }
}

using FluentValidation;
using CardManagement.Shared.DTOs;

namespace MerchantApp.API.Validators
{
    // Validator for merchant login requests
    public class MerchantLoginDTOValidator : AbstractValidator<MerchantLoginRequestDTO>
    {
        public MerchantLoginDTOValidator()
        {
            // Email must not be empty and must be a valid email format
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email cannot be empty.")
                .EmailAddress().WithMessage("Invalid email format.");

            // Password must not be empty
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password cannot be empty.");
        }
    }
}

using FluentValidation;
using CardManagement.Shared.DTOs;
using System.Linq;

namespace MerchantApp.API.Validators
{
    // Validator for payment status DTOs
    public class PaymentStatusDTOValidator : AbstractValidator<PaymentStatusDTO>
    {
        public PaymentStatusDTOValidator()
        {
            // TransactionId must be greater than 0
            RuleFor(x => x.TransactionId)
                .GreaterThan(0).WithMessage("Invalid Transaction ID.");

            // Status must not be empty and must be one of the allowed values
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status cannot be empty.")
                .Must(s => new[] { "Pending", "Success", "Failed", "Timeout" }.Contains(s))
                .WithMessage("Invalid status value.");
        }
    }
}

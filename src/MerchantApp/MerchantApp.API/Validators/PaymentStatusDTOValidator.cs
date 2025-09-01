using FluentValidation;

namespace MerchantApp.API.Validators
{
    /// Ensures that transaction ID and status values are valid.
    public class PaymentStatusDTOValidator : AbstractValidator<DTOs.PaymentStatusDTO>
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

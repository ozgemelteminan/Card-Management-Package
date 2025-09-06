using FluentValidation;
using CardManagement.Shared.DTOs;

namespace MerchantApp.API.Validators
{
    // Validator for QR code payment DTOs
    public class QRCodePaymentDTOValidator : AbstractValidator<QRCodePaymentDTO>
    {
        public QRCodePaymentDTOValidator()
        {
            // MerchantId must be greater than 0
            RuleFor(x => x.MerchantId)
                .GreaterThan(0).WithMessage("Invalid Merchant ID.");

            // TotalAmount must be greater than 0
            RuleFor(x => x.TotalAmount)
                .GreaterThan(0).WithMessage("Total amount must be greater than 0.");

            // Cart items cannot be empty
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Cart cannot be empty.");

            // Validate each cart item using the CartItemDTOValidator
            RuleForEach(x => x.Items).SetValidator(new CartItemDTOValidator());
        }
    }
}

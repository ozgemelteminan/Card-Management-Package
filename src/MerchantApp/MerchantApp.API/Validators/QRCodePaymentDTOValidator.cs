using FluentValidation;
using MerchantApp.API.DTOs;

namespace MerchantApp.API.Validators
{
    /// <summary>
    /// Validator for QRCodePaymentDTO.
    /// Ensures that MerchantId, TotalAmount, and Items are valid before processing QR code payments.
    /// </summary>
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

            // Items collection must not be empty
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Cart cannot be empty.");

            // Each item in the cart must be validated with CartItemDTOValidator
            RuleForEach(x => x.Items).SetValidator(new CartItemDTOValidator());
        }
    }
}

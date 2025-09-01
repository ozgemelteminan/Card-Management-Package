using FluentValidation;

namespace MerchantApp.API.Validators
{
    // Validator for the CartDTO object
    public class CartDTOValidator : AbstractValidator<DTOs.CartDTO>
    {
        public CartDTOValidator()
        {
            // Rule: MerchantId must be greater than 0 (a valid ID)
            RuleFor(x => x.MerchantId)
                .GreaterThan(0).WithMessage("Invalid Merchant ID.");

            // Rule: The cart must not be empty
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Cart cannot be empty.");

            // Rule: For each item in Items, apply the CartItemDTOValidator
            RuleForEach(x => x.Items)
                .SetValidator(new CartItemDTOValidator());
        }
    }
}

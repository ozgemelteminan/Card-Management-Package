using FluentValidation;
using CardManagement.Shared.DTOs;
using CardManagement.Shared.Models; 

namespace MerchantApp.API.Validators
{
    // Validator for CartDTO using FluentValidation
    public class CartDTOValidator : AbstractValidator<CartDTO>
    {
        public CartDTOValidator()
        {
            // MerchantId must be greater than 0
            RuleFor(x => x.MerchantId)
                .GreaterThan(0).WithMessage("Invalid Merchant ID.");

            // The cart must have at least one item
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Cart cannot be empty.");

            // Each item in the cart is validated using CartItemDTOValidator
            RuleForEach<CartItemDTO>(x => x.Items)
                .SetValidator(new CartItemDTOValidator());
        }
    }
}

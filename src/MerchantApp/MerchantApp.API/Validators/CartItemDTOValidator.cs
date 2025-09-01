using FluentValidation;
using MerchantApp.API.DTOs;

namespace MerchantApp.API.Validators
{
    // Validator for the CartItemDTO object
    public class CartItemDTOValidator : AbstractValidator<CartItemDTO>
    {
        public CartItemDTOValidator()
        {
            // Rule: ProductId must be greater than 0 (valid product)
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Invalid Product ID.");

            // Rule: Quantity must be greater than 0
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        }
    }
}

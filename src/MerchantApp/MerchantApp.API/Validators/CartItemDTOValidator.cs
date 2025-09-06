using FluentValidation;
using CardManagement.Shared.DTOs;

namespace MerchantApp.API.Validators
{
    // Validator for individual cart items
    public class CartItemDTOValidator : AbstractValidator<CartItemDTO>
    {
        public CartItemDTOValidator()
        {
            // ProductId must be greater than 0
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Invalid Product ID.");

            // Quantity must be greater than 0
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        }
    }
}

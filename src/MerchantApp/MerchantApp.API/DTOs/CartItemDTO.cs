namespace MerchantApp.API.DTOs;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a single item inside a cart (used when creating or updating a cart).
/// </summary>
public class CartItemDTO
{
    /// <summary>
    /// ID of the product being added to the cart.
    /// </summary>
    [Required]
    public int ProductId { get; set; }

    /// <summary>
    /// Number of units for this product.
    /// </summary>
    [Required]
    public int Quantity { get; set; }
}

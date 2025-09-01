namespace MerchantApp.API.Models;

public class CartItem
{
    // Primary Key: unique ID for each item in the cart
    public int CartItemId { get; set; }

    // The merchant (store/owner) associated with this cart
    public int MerchantId { get; set; }

    // The product being added to the cart (linked to Product table)
    public int ProductId { get; set; }

    // How many units of the product were added
    public int Quantity { get; set; }

    // Price per single unit of the product at the time of adding to cart
    public decimal UnitPrice { get; set; }

    // Calculated field: Quantity * UnitPrice
    public decimal TotalPrice { get; set; }
}

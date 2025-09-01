namespace MerchantApp.API.Models;

public class Product
{
    // Primary Key: unique ID for each product
    public int ProductId { get; set; }

    // Foreign Key: links this product to its merchant (owner)
    public int MerchantId { get; set; }

    // Name of the product
    public string Name { get; set; } = null!;

    // Price of the product
    public decimal Price { get; set; }

    // Stock quantity (how many units are available for sale)
    public int Stock { get; set; }   
    
    // Timestamp when the product was created/added
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property: reference to the merchant who owns this product
    public Merchant Merchant { get; set; } = null!;
}

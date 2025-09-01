namespace MerchantApp.API.Models;

public class TransactionProductDetail
{
    // Primary Key: unique ID for each transaction line item
    public int TransactionProductDetailId { get; set; }

    // Foreign Key: links this detail record to its parent Transaction
    public int TransactionId { get; set; }

    // Foreign Key: which product was purchased
    public int ProductId { get; set; }

    // How many units of the product were purchased
    public int Quantity { get; set; }

    // Price of a single unit at the time of transaction
    public decimal UnitPrice { get; set; }

    // Total price for this product line (Quantity × UnitPrice)
    public decimal TotalPrice { get; set; }

    // Navigation property: parent Transaction (many details → one transaction)
    public Transaction Transaction { get; set; } = null!;

    // Navigation property: related Product
    public Product Product { get; set; } = null!;
}

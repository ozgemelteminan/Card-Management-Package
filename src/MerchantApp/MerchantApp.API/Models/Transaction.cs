namespace MerchantApp.API.Models;

public class Transaction
{
    // Primary Key: unique ID for each transaction
    public int TransactionId { get; set; }

    // Foreign Key: which merchant started this transaction
    public int MerchantId { get; set; }

    // Foreign Key: which card was used for payment (null until cardholder approves)
    public int? CardId { get; set; } // Set after QR scan & approval

    // Total amount of the transaction (sum of all product prices in cart)
    public decimal TotalAmount { get; set; }

    // Current status of the transaction: Pending / Success / Failed / Timeout
    public string Status { get; set; } = "Pending"; 

    // Timestamp when the transaction was created
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Timeout: transaction expires if not approved within 45 seconds
    public DateTime? ExpiresAt { get; set; }

    // Timestamp for last update (e.g., when payment is completed or failed)
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;


    // Navigation properties (relationships)
    
    // Link to the merchant who owns this transaction
    public Merchant Merchant { get; set; } = null!;

    // Link to the card used in the transaction (if any)
    public Card? Card { get; set; }

    // List of products included in this transaction
    public ICollection<TransactionProductDetail> ProductDetails { get; set; } 
        = new List<TransactionProductDetail>();
}

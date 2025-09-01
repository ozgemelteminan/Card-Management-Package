namespace MerchantApp.API.Models;

public class Merchant
{
    // Primary Key: unique ID for each merchant (store/partner)
    public int MerchantId { get; set; }

    // Merchant’s business or personal name
    public string Name { get; set; } = null!;

    // Email used for login/communication
    public string Email { get; set; } = null!;

    // Hashed password for authentication (never store plain text passwords)
    public string PasswordHash { get; set; } = null!;

    // Date when the merchant was created/registered in the system
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Last updated timestamp (optional, null if never updated)
    public DateTime? UpdatedAt { get; set; }

    // One-to-many relationship: a merchant can have multiple products
    public ICollection<Product> Products { get; set; } = new List<Product>();

    // One-to-many relationship: a merchant can have multiple transactions
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

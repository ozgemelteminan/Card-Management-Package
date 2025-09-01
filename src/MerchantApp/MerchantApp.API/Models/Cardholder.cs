namespace MerchantApp.API.Models;

public class Cardholder
{
    // Primary Key: unique ID for each cardholder
    public int CardholderId { get; set; }

    // Full name of the cardholder (e.g., "John Doe")
    public string FullName { get; set; } = null!;

    // Email address of the cardholder (used for login/communication)
    public string Email { get; set; } = null!;

    // Hashed password for authentication (never store plain text passwords)
    public string PasswordHash { get; set; } = null!;

    // Timestamp when the cardholder account was created
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Property: list of cards owned by this cardholder
    public ICollection<Card> Cards { get; set; } = new List<Card>();
}

namespace MerchantApp.API.Models;

public class Card
{
    // Primary Key: unique ID for each card
    public int CardId { get; set; }

    // Foreign Key: links the card to its owner (Cardholder)
    public int CardholderId { get; set; }

    // 16-digit card number (e.g., "4111111111111111")
    public string CardNumber { get; set; } = null!;

    // Expiry date in "MM/YY" format (e.g., "12/27")
    public string ExpiryDate { get; set; } = null!;

    // 3-digit security code (usually printed on the back of the card)
    public string CVV { get; set; } = null!;

    // 4-digit PIN code (used for authentication)
    public string Pin { get; set; } = null!;

    // Balance of the card (how much money is available)
    public decimal Balance { get; set; }

    // Navigation Property: reference to the card owner (1 card belongs to 1 cardholder)
    public Cardholder Cardholder { get; set; } = null!;

    // Navigation Property: all transactions made with this card (1 card can have many transactions)
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

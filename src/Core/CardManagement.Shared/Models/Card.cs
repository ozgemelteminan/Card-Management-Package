namespace CardManagement.Shared.Models;
public class Card
{
    public int CardId { get; set; }
    public int CardholderId { get; set; }
    public string CardNumber { get; set; } = null!;
    public string ExpiryDate { get; set; } = null!;
    public string CVV { get; set; } = null!;
    public string Pin { get; set; } = null!;
    public decimal Balance { get; set; }
    public Cardholder Cardholder { get; set; } = null!;
    

    // Navigation Property: all transactions made with this card (1 card can have many transactions)
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

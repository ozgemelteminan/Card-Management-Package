namespace CardManagement.Shared.Models;

public class Cardholder
{
    public int CardholderId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Card> Cards { get; set; } = new List<Card>();
}

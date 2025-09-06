namespace CardManagement.Shared.DTOs
{
    public class CardCreateDTO
    {
        public int CardholderId { get; set; }  
        public string CardNumber { get; set; } = null!;
        public string ExpiryDate { get; set; } = null!;
        public string CVV { get; set; } = null!;
        public string Pin { get; set; } = null!;
        public decimal Balance { get; set; }
    }
}

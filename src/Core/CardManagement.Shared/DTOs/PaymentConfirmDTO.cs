namespace CardManagement.Shared.DTOs
{
    public class PaymentConfirmDTO
    {
        public int TransactionId { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public string ExpiryDate { get; set; } = string.Empty;
        public string CVV { get; set; } = string.Empty;
        public string Pin { get; set; } = string.Empty;
    }


    public class PaymentStatusDTO
    {
        public int TransactionId { get; set; }
        public string Status { get; set; } = "";
        public decimal TotalAmount { get; set; }
    }
}

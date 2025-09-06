namespace CardManagement.Shared.DTOs
{
    public class TransactionStatusDTO
    {
        public int TransactionId { get; set; }
        public string Status { get; set; } = null!;
        public string? Reason { get; set; }
    }

    public class StartTransactionResponse
    {
        public int TransactionId { get; set; }
        public bool Success { get; set; }
    }

}

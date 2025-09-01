namespace Merchant.API.Models
{
    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class TransactionSummaryDto
    {
        public int TotalCount { get; set; }
        public decimal TotalAmount { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        // İstersen TimeoutCount da ekleyebilirsin
        public int TimeoutCount { get; set; }
    }



    public static class TxStatus
    {
        public const string Pending = "Pending";
        public const string Success = "Success";
        public const string Failed = "Failed";
        public const string Timeout = "Timeout";
    }
}

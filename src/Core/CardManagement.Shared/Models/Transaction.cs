using System;
using System.Collections.Generic;

namespace CardManagement.Shared.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int MerchantId { get; set; }
        public int? CardId { get; set; } 
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Merchant? Merchant { get; set; } 
        public Card? Card { get; set; }         
        public ICollection<TransactionProductDetail> ProductDetails { get; set; } 
            = new List<TransactionProductDetail>();
    }
}

using System;
using System.Collections.Generic;

namespace CardManagement.Shared.Models
{
    public class Merchant
    {
        public int MerchantId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
         public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }

        // Navigation properties
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}

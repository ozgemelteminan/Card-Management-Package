using System;

namespace CardManagement.Shared.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public int MerchantId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Merchant? Merchant { get; set; }
    }
}

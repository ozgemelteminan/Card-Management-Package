using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardManagement.Data.Entities
{
    [Table("Transactions")]
    public class TransactionEntity
    {
        [Key]
        public int TransactionId { get; set; } // PK

        [ForeignKey(nameof(MerchantId))]
        public int MerchantId { get; set; } 

        
        public int CardId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending/Success/Failed/Timeout

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

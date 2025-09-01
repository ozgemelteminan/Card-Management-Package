using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardManagement.Data.Entities
{
    [Table("Products")]
    public class ProductEntity
    {
        [Key]
        public int ProductId { get; set; } 

        [ForeignKey(nameof(MerchantId))]
        public int MerchantId { get; set; } 

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

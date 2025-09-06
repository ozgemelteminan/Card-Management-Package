using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CardManagement.Shared.DTOs
{
    public class CartDTO
    {
        public int MerchantId { get; set; }
        public List<CartItemDTO> Items { get; set; } = new();
    }
}

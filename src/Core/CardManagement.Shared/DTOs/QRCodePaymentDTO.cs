using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CardManagement.Shared.DTOs
{
    public class QRCodePaymentDTO
    {
        public int MerchantId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<CartItemDTO> Items { get; set; } = new();
        public int? ExpireSeconds { get; set; }
    }
    
    
        public class QRCodePaymentResponseDTO
    {
        public int TransactionId { get; set; }
        public string QrPayload { get; set; } = string.Empty;
        public string? QrCodeBase64 { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string? Status { get; set; }
    }
}

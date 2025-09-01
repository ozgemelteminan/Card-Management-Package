namespace MerchantApp.API.DTOs
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Data Transfer Object (DTO) used to return the QR Code payment response.
    /// Contains the transaction details and the generated QR payload.
    /// </summary>
    public class QRCodePaymentResponseDTO
    {
        /// <summary>
        /// The unique identifier of the transaction.
        /// </summary>
        [Required]
        public int TransactionId { get; set; }

        /// <summary>
        /// The QR payload that can be encoded into a QR code.
        /// Typically includes transaction information to be scanned by the customer.
        /// </summary>
        [Required]
        public string QrPayload { get; set; } = "";

        /// <summary>
        /// The expiration time of the QR code (e.g., 45 seconds from initiation).
        /// After this time, the QR code becomes invalid.
        /// </summary>
        [Required]
        public DateTime ExpiresAt { get; set; }
    }
}

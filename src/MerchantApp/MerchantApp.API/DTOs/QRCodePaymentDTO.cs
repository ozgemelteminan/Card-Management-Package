namespace MerchantApp.API.DTOs;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// DTO used when initiating a QR code based payment request.
/// The merchant sends basket details and receives a QR payload in return.
/// </summary>
public class QRCodePaymentDTO
{
    /// <summary>
    /// Merchant ID that owns the transaction.
    /// </summary>
    [Required]
    public int MerchantId { get; set; }

    /// <summary>
    /// Total amount of the basket to be paid.
    /// </summary>
    [Required]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Items in the basket (ProductId, Quantity, UnitPrice etc.).
    /// </summary>
    [Required]
    public List<CartItemDTO> Items { get; set; } = new();

    /// <summary>
    /// Expiry duration in seconds for the QR code (optional).
    /// Example: 45 seconds.
    /// </summary>
    [Required]
    public int? ExpireSeconds { get; set; }
}

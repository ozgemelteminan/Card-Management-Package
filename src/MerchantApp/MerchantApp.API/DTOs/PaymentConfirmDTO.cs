namespace MerchantApp.API.DTOs;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// DTO used when a cardholder confirms a payment with their card details.
/// </summary>
public class PaymentConfirmDTO
{
    /// <summary>
    /// The transaction ID that is being confirmed.
    /// </summary>
    [Required]
    public int TransactionId { get; set; }

    /// <summary>
    /// The card number (typically 16 digits).
    /// </summary>
    [Required]
    public string CardNumber { get; set; } = null!;

    /// <summary>
    /// Expiry date of the card (format: MM/YY).
    /// </summary>
    [Required]
    public string ExpiryDate { get; set; } = null!;

    /// <summary>
    /// 3-digit CVV code of the card.
    /// </summary>
    [Required]
    public string CVV { get; set; } = null!;

    /// <summary>
    /// 4-digit PIN code of the card.
    /// </summary>
    [Required]
    public string Pin { get; set; } = null!;
}

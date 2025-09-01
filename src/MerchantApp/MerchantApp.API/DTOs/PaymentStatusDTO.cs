namespace MerchantApp.API.DTOs;
using System.ComponentModel.DataAnnotations;


/// <summary>
/// DTO used to represent the status of a payment transaction.
/// </summary>
public class PaymentStatusDTO
{
    /// <summary>
    /// Unique identifier of the transaction.
    /// </summary>
    [Required]
    public int TransactionId { get; set; }

    /// <summary>
    /// Current status of the transaction. 
    /// Possible values: "Pending", "Success", "Failed", "Timeout".
    /// </summary>
    [Required]
    public string Status { get; set; } = "";

    /// <summary>
    /// Total amount involved in the transaction.
    /// </summary>
    [Required]
    public decimal TotalAmount { get; set; }
}

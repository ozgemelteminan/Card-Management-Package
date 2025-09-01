namespace MerchantApp.API.DTOs;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// DTO used when a merchant tries to log in.
/// </summary>
public class MerchantLoginRequestDTO
{
    /// <summary>
    /// Merchant's registered email address.
    /// </summary>
    [Required]
    public string Email { get; set; } = "";

    /// <summary>
    /// Hashed password (client should send hash, not plain text).
    /// </summary>
    [Required]
    public string PasswordHash { get; set; } = "";
}

/// <summary>
/// Response DTO returned after successful merchant login.
/// </summary>
public class MerchantLoginResponseDTO
{
    /// <summary>
    /// JWT authentication token.
    /// </summary>
    [Required]
    public string Token { get; set; } = "";

    /// <summary>
    /// Unique merchant identifier.
    /// </summary>
    [Required]
    public int MerchantId { get; set; }

    /// <summary>
    /// Merchant's display name.
    /// </summary>
    [Required]
    public string MerchantName { get; set; } = "";

    /// <summary>
    /// Token expiration date and time (UTC).
    /// </summary>
    [Required]
    public DateTime ExpiresAt { get; set; }
}

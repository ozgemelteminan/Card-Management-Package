using System.ComponentModel.DataAnnotations;

namespace MerchantApp.API.DTOs
{
    /// <summary>
    /// DTO used when a new merchant registers to the system.
    /// </summary>
    public class MerchantRegisterDTO
    {
        /// <summary>
        /// Merchant's business or personal name.
        /// </summary>
        [Required]
        public required string Name { get; set; }

        /// <summary>
        /// Merchant's email address (used for login).
        /// </summary>
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        /// <summary>
        /// Password for the merchant account (will be stored as hash).
        /// </summary>
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public required string Password { get; set; }
    }
}

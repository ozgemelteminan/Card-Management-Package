namespace MerchantApp.API.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating a new cardholder.
    /// </summary>
    public class CardholderCreateDTO
    {
        /// <summary>
        /// Full name of the cardholder.
        /// </summary>
        public string FullName { get; set; } = null!;

        /// <summary>
        /// Email address of the cardholder (must be unique).
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Password in plain text (will be hashed before storing in DB).
        /// </summary>
        public string PasswordHash { get; set; } = null!; 
    }
}

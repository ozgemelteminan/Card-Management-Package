namespace MerchantApp.API.DTOs
{
    /// <summary>
    /// Response model returned when fetching cardholder information.
    /// </summary>
    public class CardholderResponseDTO
    {
        /// <summary>
        /// Unique identifier of the cardholder.
        /// </summary>
        public int CardholderId { get; set; }

        /// <summary>
        /// Full name of the cardholder.
        /// </summary>
        public string FullName { get; set; } = null!;

        /// <summary>
        /// Email address of the cardholder.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Date and time when the cardholder was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}

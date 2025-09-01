namespace MerchantApp.API.DTOs
{
    /// <summary>
    /// DTO for creating a new card for a cardholder.
    /// </summary>
    public class CardCreateDTO
    {
        /// <summary>
        /// The ID of the cardholder that owns this card.
        /// </summary>
        public int CardholderId { get; set; }  

        /// <summary>
        /// Card number (16 digits).
        /// </summary>
        public string CardNumber { get; set; } = null!;

        /// <summary>
        /// Expiry date of the card in MM/YY format.
        /// </summary>
        public string ExpiryDate { get; set; } = null!;

        /// <summary>
        /// Card verification value (3 digits).
        /// </summary>
        public string CVV { get; set; } = null!;

        /// <summary>
        /// 4-digit PIN for the card.
        /// </summary>
        public string Pin { get; set; } = null!;

        /// <summary>
        /// Initial balance of the card.
        /// </summary>
        public decimal Balance { get; set; }
    }
}

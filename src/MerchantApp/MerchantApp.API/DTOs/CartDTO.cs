using System.ComponentModel.DataAnnotations;

namespace MerchantApp.API.DTOs
{
    /// <summary>
    /// Represents a shopping cart request containing the merchant ID and the list of items.
    /// </summary>
    public class CartDTO
    {
        /// <summary>
        /// The unique ID of the merchant who owns the cart.
        /// </summary>
        [Required]
        public int MerchantId { get; set; }

        /// <summary>
        /// The list of items in the cart.
        /// </summary>
        [Required]
        public List<CartItemDTO> Items { get; set; } = new();
    }
}

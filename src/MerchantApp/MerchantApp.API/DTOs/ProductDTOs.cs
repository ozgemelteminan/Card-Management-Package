namespace MerchantApp.API.DTOs
{
    /// <summary>
    /// DTO used when creating a new product from the client side.
    /// </summary>
    public class ProductCreateDTO
    {
        /// <summary>
        /// Name of the product.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Price of the product.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Stock quantity available for the product.
        /// </summary>
        public int Stock { get; set; }   
    }

    /// <summary>
    /// DTO returned to the client after product creation or fetching product details.
    /// </summary>
    public class ProductResponseDTO
    {
        /// <summary>
        /// Unique identifier for the product.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Name of the product.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Price of the product.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Date and time when the product was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Identifier of the merchant who owns the product.
        /// </summary>
        public int MerchantId { get; set; }
    }
}

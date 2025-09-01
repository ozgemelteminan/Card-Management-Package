using MerchantApp.API.DTOs;

namespace MerchantApp.API.Services.Abstractions;

/// <summary>
/// Operations for managing a merchant's cart. Stock is reserved when items are added,
/// and released when items are removed or the cart is cleared.
/// </summary>
public interface ICartService
{
    Task<CartDTO> GetAsync(int merchantId, CancellationToken ct = default);
    Task<CartDTO> AddItemAsync(int merchantId, int productId, int quantity, CancellationToken ct = default);
    Task<CartDTO> RemoveItemAsync(int merchantId, int productId, int? quantity = null, CancellationToken ct = default);
    Task ClearAsync(int merchantId, CancellationToken ct = default);
}

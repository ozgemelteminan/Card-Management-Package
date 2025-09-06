using System.Collections.Generic;
using System.Threading.Tasks;
using CardManagement.Shared.DTOs;

namespace CardManagement.Shared.Interfaces
{
    public interface ICartService
    {
        Task AddItemAsync(int merchantId, CartItemDTO item);
        Task<List<CartItemDTO>> GetCartAsync(int merchantId);
        Task RemoveItemAsync(int merchantId, int productId);
        Task ClearCartAsync(int merchantId);
    }
}

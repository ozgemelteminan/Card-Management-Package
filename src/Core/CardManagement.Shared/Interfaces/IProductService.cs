using System.Collections.Generic;
using System.Threading.Tasks;
using CardManagement.Shared.DTOs;
using CardManagement.Shared.Models;

namespace MerchantApp.Service.Services
{
    public interface IProductService
    {
        Task<Product> AddProductAsync(int merchantId, ProductCreateDTO dto);
        Task<List<Product>> GetProductsByMerchantAsync(int merchantId);
        Task<bool> DeleteProductAsync(int merchantId, int productId);
    }
}

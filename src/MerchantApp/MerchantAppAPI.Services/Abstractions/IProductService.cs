using MerchantApp.API.DTOs;

namespace MerchantApp.API.Services.Abstractions;

public interface IProductService
{
    Task<ProductDetailDTO> CreateAsync(int merchantId, ProductCreateDTO dto, CancellationToken ct = default);
    Task<ProductDetailDTO?> UpdateAsync(int merchantId, int productId, ProductUpdateDTO dto, CancellationToken ct = default);
    Task DeleteAsync(int merchantId, int productId, CancellationToken ct = default);
    Task<IReadOnlyList<ProductDetailDTO>> ListAsync(int merchantId, CancellationToken ct = default);
    Task<ProductDetailDTO?> GetAsync(int merchantId, int productId, CancellationToken ct = default);
}

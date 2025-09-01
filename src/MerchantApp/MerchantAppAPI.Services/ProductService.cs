using MerchantApp.API.Data;
using MerchantApp.API.DTOs;
using MerchantApp.API.Models;
using MerchantApp.API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MerchantApp.API.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db) => _db = db;

    public async Task<ProductDetailDTO> CreateAsync(int merchantId, ProductCreateDTO dto, CancellationToken ct = default)
    {
        var product = new Product
        {
            MerchantId = merchantId,
            Name = dto.Name,
            Price = dto.Price,
            Stock = dto.Stock,
            CreatedAt = DateTime.UtcNow
        };
        await _db.Products.AddAsync(product, ct);
        await _db.SaveChangesAsync(ct);

        return ToDetailDto(product);
    }

    public async Task<ProductDetailDTO?> UpdateAsync(int merchantId, int productId, ProductUpdateDTO dto, CancellationToken ct = default)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == productId && p.MerchantId == merchantId, ct);
        if (product is null) return null;

        product.Name = dto.Name ?? product.Name;
        if (dto.Price.HasValue) product.Price = dto.Price.Value;
        if (dto.Stock.HasValue) product.Stock = dto.Stock.Value;
        product.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return ToDetailDto(product);
    }

    public async Task DeleteAsync(int merchantId, int productId, CancellationToken ct = default)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == productId && p.MerchantId == merchantId, ct);
        if (product is null) throw new KeyNotFoundException("Product not found.");

        _db.Products.Remove(product);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ProductDetailDTO>> ListAsync(int merchantId, CancellationToken ct = default)
    {
        return await _db.Products
            .Where(p => p.MerchantId == merchantId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => ToDetailDto(p))
            .ToListAsync(ct);
    }

    public async Task<ProductDetailDTO?> GetAsync(int merchantId, int productId, CancellationToken ct = default)
    {
        return await _db.Products
            .Where(p => p.MerchantId == merchantId && p.ProductId == productId)
            .Select(p => ToDetailDto(p))
            .FirstOrDefaultAsync(ct);
    }

    private static ProductDetailDTO ToDetailDto(Product p) => new()
    {
        ProductId = p.ProductId,
        Name = p.Name,
        Price = p.Price,
        CreatedAt = p.CreatedAt,
        MerchantId = p.MerchantId,
        Stock = p.Stock
    };
}

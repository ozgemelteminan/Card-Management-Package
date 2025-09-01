using MerchantApp.API.Data;
using MerchantApp.API.DTOs;
using MerchantApp.API.Models;
using MerchantApp.API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MerchantApp.API.Services;

public class CartService : ICartService
{
    private readonly AppDbContext _db;

    public CartService(AppDbContext db) => _db = db;

    public async Task<CartDTO> GetAsync(int merchantId, CancellationToken ct = default)
    {
        var items = await _db.CartItems
            .Where(c => c.MerchantId == merchantId)
            .Select(c => new CartItemDTO { ProductId = c.ProductId, Quantity = c.Quantity })
            .ToListAsync(ct);

        return new CartDTO { MerchantId = merchantId, Items = items };
    }

    public async Task<CartDTO> AddItemAsync(int merchantId, int productId, int quantity, CancellationToken ct = default)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

        var product = await _db.Products
            .FirstOrDefaultAsync(p => p.ProductId == productId && p.MerchantId == merchantId, ct);
        if (product is null) throw new KeyNotFoundException("Product not found or not owned by merchant.");

        if (product.Stock < quantity) throw new InvalidOperationException("Insufficient stock.");

        var item = await _db.CartItems.FirstOrDefaultAsync(c => c.MerchantId == merchantId && c.ProductId == productId, ct);
        if (item is null)
        {
            item = new CartItem
            {
                MerchantId = merchantId,
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = product.Price,
                TotalPrice = product.Price * quantity
            };
            await _db.CartItems.AddAsync(item, ct);
        }
        else
        {
            item.Quantity += quantity;
            item.UnitPrice = product.Price; // lock current price
            item.TotalPrice = item.Quantity * item.UnitPrice;
        }

        // reserve stock
        product.Stock -= quantity;

        await _db.SaveChangesAsync(ct);
        return await GetAsync(merchantId, ct);
    }

    public async Task<CartDTO> RemoveItemAsync(int merchantId, int productId, int? quantity = null, CancellationToken ct = default)
    {
        var item = await _db.CartItems.FirstOrDefaultAsync(c => c.MerchantId == merchantId && c.ProductId == productId, ct);
        if (item is null) throw new KeyNotFoundException("Cart item not found.");

        var product = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == productId && p.MerchantId == merchantId, ct);
        if (product is null) throw new KeyNotFoundException("Product not found.");

        int removeQty = quantity ?? item.Quantity;
        if (removeQty <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

        if (removeQty >= item.Quantity)
        {
            // restore full quantity
            product.Stock += item.Quantity;
            _db.CartItems.Remove(item);
        }
        else
        {
            item.Quantity -= removeQty;
            product.Stock += removeQty;
            item.TotalPrice = item.Quantity * item.UnitPrice;
        }

        await _db.SaveChangesAsync(ct);
        return await GetAsync(merchantId, ct);
    }

    public async Task ClearAsync(int merchantId, CancellationToken ct = default)
    {
        var items = await _db.CartItems.Where(c => c.MerchantId == merchantId).ToListAsync(ct);

        if (items.Count == 0) return;

        var productIds = items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _db.Products
            .Where(p => productIds.Contains(p.ProductId) && p.MerchantId == merchantId)
            .ToDictionaryAsync(p => p.ProductId, ct);

        foreach (var i in items)
        {
            if (products.TryGetValue(i.ProductId, out var p))
            {
                p.Stock += i.Quantity; // restore
            }
        }

        _db.CartItems.RemoveRange(items);
        await _db.SaveChangesAsync(ct);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using CardManagement.Shared.DTOs;
using CardManagement.Shared.Interfaces;
using CardManagement.Data;

namespace MerchantApp.Service.Services
{
    public class CartService : ICartService
    {
        private readonly IMemoryCache _cache; // In-memory cache for storing cart items
        private readonly AppDbContext _db;    // DB context, can be used if needed in the future

        // Cache entry options with 45 minutes sliding expiration
        private readonly MemoryCacheEntryOptions _options = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(45)
        };

        public CartService(IMemoryCache cache, AppDbContext db)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache)); // Throw if cache is null
            _db = db ?? throw new ArgumentNullException(nameof(db));           // Throw if db is null
        }

        // Generate cache key for merchant
        private string Key(int merchantId) => $"cart:{merchantId}";

        // Add item to merchant's cart
        public Task AddItemAsync(int merchantId, CartItemDTO item)
        {
            // Get existing cart or create new list
            var list = _cache.GetOrCreate(Key(merchantId), _ => new List<CartItemDTO>())!;

            // If product exists, increase quantity; otherwise add new item
            var existing = list.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
            }
            else
            {
                list.Add(new CartItemDTO
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            // Update cache with new list
            _cache.Set(Key(merchantId), list, _options);
            return Task.CompletedTask;
        }

        // Get all items in merchant's cart
        public Task<List<CartItemDTO>> GetCartAsync(int merchantId)
        {
            var list = _cache.GetOrCreate(Key(merchantId), _ => new List<CartItemDTO>())!;
            return Task.FromResult(list);
        }

        // Remove a specific product from cart
        public Task RemoveItemAsync(int merchantId, int productId)
        {
            var list = _cache.GetOrCreate(Key(merchantId), _ => new List<CartItemDTO>())!;
            list.RemoveAll(i => i.ProductId == productId);

            _cache.Set(Key(merchantId), list, _options);
            return Task.CompletedTask;
        }

        // Clear all items from merchant's cart
        public Task ClearCartAsync(int merchantId)
        {
            _cache.Remove(Key(merchantId));
            return Task.CompletedTask;
        }
    }
}

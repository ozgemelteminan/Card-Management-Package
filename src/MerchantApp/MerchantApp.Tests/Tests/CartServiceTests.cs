using Xunit;
using MerchantApp.Service.Services;
using Microsoft.Extensions.Caching.Memory;
using CardManagement.Shared.DTOs;
using CardManagement.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace MerchantApp.Tests.Tests
{
    public class CartServiceTests
    {
        // Helper method to create a CartService with in-memory DB and cache
        private CartService CreateService()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Each test uses a unique DB
                .Options;

            var db = new AppDbContext(options);
            var cache = new MemoryCache(new MemoryCacheOptions());

            return new CartService(cache, db);
        }

        [Fact]
        public async Task AddItem_ShouldStoreItemInCache()
        {
            // Test that adding a new item stores it in the cache

            var service = CreateService();
            var item = new CartItemDTO { ProductId = 1, Quantity = 2 };

            await service.AddItemAsync(1, item); // Add item for merchantId=1
            var cart = await service.GetCartAsync(1);

            // Assert that the cart contains exactly 1 item with the correct quantity
            Assert.Single(cart);
            Assert.Equal(2, cart[0].Quantity);
        }

        [Fact]
        public async Task AddItem_ShouldIncreaseQuantity_WhenItemExists()
        {
            // Test that adding the same item again increases the quantity

            var service = CreateService();
            await service.AddItemAsync(1, new CartItemDTO { ProductId = 1, Quantity = 2 });

            // Add same product again
            await service.AddItemAsync(1, new CartItemDTO { ProductId = 1, Quantity = 3 });
            var cart = await service.GetCartAsync(1);

            // Assert that quantity is summed correctly
            Assert.Single(cart);
            Assert.Equal(5, cart[0].Quantity);
        }

        [Fact]
        public async Task RemoveItem_ShouldRemoveItemFromCache()
        {
            // Test that removing an item actually deletes it from the cache

            var service = CreateService();
            await service.AddItemAsync(1, new CartItemDTO { ProductId = 1, Quantity = 2 });
            await service.AddItemAsync(1, new CartItemDTO { ProductId = 2, Quantity = 1 });

            await service.RemoveItemAsync(1, 1); // Remove productId=1
            var cart = await service.GetCartAsync(1);

            // Assert that only productId=2 remains
            Assert.Single(cart);
            Assert.Equal(2, cart[0].ProductId);
        }

        [Fact]
        public async Task ClearCart_ShouldEmptyCache()
        {
            // Test that clearing the cart removes all items

            var service = CreateService();
            await service.AddItemAsync(1, new CartItemDTO { ProductId = 1, Quantity = 2 });
            await service.AddItemAsync(1, new CartItemDTO { ProductId = 2, Quantity = 1 });

            await service.ClearCartAsync(1); // Clear all items
            var cart = await service.GetCartAsync(1);

            // Assert that cart is empty
            Assert.Empty(cart);
        }

        [Fact]
        public async Task GetCart_ShouldReturnEmptyList_WhenNoItems()
        {
            // Test that getting a cart with no items returns an empty list

            var service = CreateService();
            var cart = await service.GetCartAsync(99); // Merchant with no items

            // Assert that cart is empty but not null
            Assert.NotNull(cart);
            Assert.Empty(cart);
        }

        [Fact]
        public async Task AddMultipleItems_ShouldStoreAllItems()
        {
            // Test that adding multiple different items stores all of them

            var service = CreateService();

            await service.AddItemAsync(1, new CartItemDTO { ProductId = 1, Quantity = 1 });
            await service.AddItemAsync(1, new CartItemDTO { ProductId = 2, Quantity = 2 });

            var cart = await service.GetCartAsync(1);

            // Assert both items exist with correct quantities
            Assert.Equal(2, cart.Count);
            Assert.Contains(cart, i => i.ProductId == 1 && i.Quantity == 1);
            Assert.Contains(cart, i => i.ProductId == 2 && i.Quantity == 2);
        }
    }
}

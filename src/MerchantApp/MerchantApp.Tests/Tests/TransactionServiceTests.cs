using Xunit;
using MerchantApp.Service.Services;
using CardManagement.Data;
using Microsoft.EntityFrameworkCore;
using CardManagement.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MerchantApp.Tests.Tests
{
    public class TransactionServiceTests
    {
        private TransactionService CreateService(out AppDbContext db)
        {
            // Create in-memory database and service instance for testing
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            db = new AppDbContext(options);
            return new TransactionService(db);
        }

        [Fact]
        public async Task StartTransactionAsync_ShouldThrow_WhenCartIsEmpty()
        {
            var service = CreateService(out var db);

            // Expect ArgumentException when starting transaction with empty cart
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await service.StartTransactionAsync(1, new List<CartItemDTO>()));
        }

        [Fact]
        public async Task StartTransactionAsync_ShouldThrow_WhenProductNotFound()
        {
            var service = CreateService(out var db);

            // Product does not exist in DB, should throw InvalidOperationException
            var items = new List<CartItemDTO>
            {
                new CartItemDTO { ProductId = 999, Quantity = 1, UnitPrice = 10 }
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await service.StartTransactionAsync(1, items));
        }

        [Fact]
        public async Task StartTransactionAsync_ShouldSucceed_WhenProductsExist()
        {
            var service = CreateService(out var db);

            // Add a product to the in-memory database
            db.Products.Add(new CardManagement.Shared.Models.Product
            {
                ProductId = 1,
                Name = "Test Product",
                Price = 100,
                Stock = 10,
                MerchantId = 1
            });
            await db.SaveChangesAsync();

            // Prepare cart items
            var items = new List<CartItemDTO>
            {
                new CartItemDTO { ProductId = 1, Quantity = 2, UnitPrice = 0 }
            };

            // Execute transaction
            var result = await service.StartTransactionAsync(1, items);

            // Assert that transaction succeeded
            Assert.True(result.Success);
            Assert.True(result.TransactionId > 0);
        }
    }
}

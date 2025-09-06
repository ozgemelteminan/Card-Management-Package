using System;
using System.Linq;
using System.Threading.Tasks;
using CardManagement.Data;
using CardManagement.Shared.DTOs;
using CardManagement.Shared.Models;
using MerchantApp.Service.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MerchantApp.Tests.Tests
{
    public class ProductServiceTests
    {
        // Helper to create new in-memory database context for each test
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddProductAsync_ShouldAddOrUpdateProduct()
        {
            // Test that adding a product works and stock increases if product exists
            var db = GetDbContext();
            var service = new ProductService(db);

            int merchantId = 1;
            var dto = new ProductCreateDTO { Name = "Test Product", Price = 10, Stock = 5 };

            // Add first time
            var result1 = await service.AddProductAsync(merchantId, dto);
            Assert.NotNull(result1);
            Assert.Equal(dto.Name, result1.Name);
            Assert.Equal(dto.Stock, result1.Stock);

            // Add same product again, stock should increase
            dto.Stock = 3;
            var result2 = await service.AddProductAsync(merchantId, dto);
            Assert.Equal(8, result2.Stock); // 5 + 3
        }

        [Fact]
        public async Task GetProductsByMerchantAsync_ShouldReturnProducts()
        {
            // Test retrieving all products for a specific merchant
            var db = GetDbContext();
            db.Products.Add(new Product { Name = "P1", MerchantId = 1, Stock = 1, Price = 10 });
            db.Products.Add(new Product { Name = "P2", MerchantId = 1, Stock = 2, Price = 20 });
            db.Products.Add(new Product { Name = "P3", MerchantId = 2, Stock = 1, Price = 15 });
            await db.SaveChangesAsync();

            var service = new ProductService(db);
            var result = await service.GetProductsByMerchantAsync(1);

            Assert.Equal(2, result.Count); // Only products with MerchantId = 1
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldRemoveProduct()
        {
            // Test that deleting a product removes it from the database
            var db = GetDbContext();
            var product = new Product { Name = "DelMe", MerchantId = 1, Stock = 1, Price = 10 };
            db.Products.Add(product);
            await db.SaveChangesAsync();

            var service = new ProductService(db);
            var success = await service.DeleteProductAsync(1, product.ProductId);

            Assert.True(success); // Delete should return true
            Assert.Empty(db.Products.Where(p => p.ProductId == product.ProductId)); // Product removed
        }
    
    }
}

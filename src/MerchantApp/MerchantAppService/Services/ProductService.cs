using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CardManagement.Data;
using CardManagement.Shared.DTOs;
using CardManagement.Shared.Models;

namespace MerchantApp.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;

        // Constructor: inject DbContext and check for null
        public ProductService(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // Add a product for a merchant or increase stock if it already exists
        public async Task<Product> AddProductAsync(int merchantId, ProductCreateDTO dto)
        {
            // Check if product with the same name already exists for this merchant
            var existingProduct = await _db.Products
                .FirstOrDefaultAsync(p => p.MerchantId == merchantId && p.Name.ToLower() == dto.Name.ToLower());

            if (existingProduct != null)
            {
                // If exists, increase stock
                existingProduct.Stock += dto.Stock;
                await _db.SaveChangesAsync();
                return existingProduct;
            }

            // Create new product entity
            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Stock = dto.Stock,
                MerchantId = merchantId,
                CreatedAt = DateTime.UtcNow
            };

            // Add to database and save
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return product;
        }

        // Get all products belonging to a merchant
        public async Task<List<Product>> GetProductsByMerchantAsync(int merchantId)
        {
            return await _db.Products
                .Where(p => p.MerchantId == merchantId)
                .ToListAsync();
        }

        // Delete a product for a merchant
        public async Task<bool> DeleteProductAsync(int merchantId, int productId)
        {
            var product = await _db.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId && p.MerchantId == merchantId);

            if (product == null) return false; // Product not found or does not belong to merchant

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MerchantApp.API.Data;
using MerchantApp.API.DTOs;
using MerchantApp.API.Models;
using System.Security.Claims;

namespace MerchantApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Only authenticated merchants can access these endpoints
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db)
    {
        _db = db;
    }

    // Helper: Extract merchantId from JWT token
    private int GetMerchantId()
    {
        var merchantIdClaim = User.FindFirstValue("merchantId");
        if (string.IsNullOrEmpty(merchantIdClaim))
            throw new UnauthorizedAccessException("merchantId not found in token.");
        return int.Parse(merchantIdClaim);
    }

    /// <summary>
    /// Add a new product for the merchant.
    /// If the product already exists (same name), the stock will be increased instead.
    /// </summary>
    [HttpPost("add")]
    public IActionResult AddProduct([FromBody] ProductCreateDTO dto)
    {
        var merchantId = GetMerchantId();

        // Check if product with the same name already exists for this merchant
        var existingProduct = _db.Products.FirstOrDefault(p =>
            p.MerchantId == merchantId && p.Name.ToLower() == dto.Name.ToLower());

        if (existingProduct != null)
        {
            existingProduct.Stock += dto.Stock;
            _db.SaveChanges();
            return Ok(new 
            { 
                Message = "Product already exists, stock updated", 
                existingProduct.ProductId, 
                existingProduct.Name, 
                existingProduct.Stock 
            });
        }

        // Add a new product
        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            Stock = dto.Stock,
            MerchantId = merchantId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Products.Add(product);
        _db.SaveChanges();

        return Ok(new 
        { 
            Message = "New product added", 
            product.ProductId, 
            product.Name, 
            product.Stock 
        });
    }

    /// <summary>
    /// Get all products that belong to the logged-in merchant.
    /// </summary>
    [HttpGet("view")]
    public IActionResult GetMyProducts()
    {
        var merchantId = GetMerchantId();

        var products = _db.Products
            .Where(p => p.MerchantId == merchantId)
            .Select(p => new
            {
                p.ProductId,
                p.Name,
                p.Price,
                p.Stock,
                p.CreatedAt
            })
            .ToList();

        return Ok(products);
    }

    /// <summary>
    /// Delete a product by ID (only if it belongs to the merchant).
    /// </summary>
    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        var merchantId = GetMerchantId();

        var product = _db.Products.FirstOrDefault(p => p.ProductId == id && p.MerchantId == merchantId);
        if (product == null)
            return NotFound("Product not found or does not belong to you.");

        _db.Products.Remove(product);
        _db.SaveChanges();

        return Ok("Product deleted.");
    }
}

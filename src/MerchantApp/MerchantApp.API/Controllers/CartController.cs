using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MerchantApp.API.DTOs;
using MerchantApp.API.Models;
using MerchantApp.API.Data;
using System.Security.Claims;

namespace MerchantApp.API.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize] // Only authorized merchants can access cart endpoints
public class CartController : ControllerBase
{
    private readonly AppDbContext _db;
    public CartController(AppDbContext db) => _db = db;

    // Extracts merchantId from JWT token claims
    private int GetMerchantId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "merchantId");
        if (claim == null) throw new UnauthorizedAccessException("No merchantId in token.");
        return int.Parse(claim.Value);
    }

    // Add a product to the merchant's cart
    [HttpPost("add")]
    public IActionResult AddToCart([FromBody] CartItemDTO dto)
    {
        var merchantId = GetMerchantId();

        // Ensure product belongs to this merchant
        var product = _db.Products.FirstOrDefault(p => p.ProductId == dto.ProductId && p.MerchantId == merchantId);
        if (product == null) return NotFound("Product not found");

        // Check stock availability
        if (product.Stock < dto.Quantity)
            return BadRequest($"Insufficient stock. Current stock: {product.Stock}");

        // Check if product already exists in cart
        var existingItem = _db.CartItems.FirstOrDefault(x => x.ProductId == product.ProductId && x.MerchantId == merchantId);

        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
            existingItem.TotalPrice = existingItem.Quantity * existingItem.UnitPrice;
        }
        else
        {
            var item = new CartItem
            {
                MerchantId = merchantId,
                ProductId = product.ProductId,
                Quantity = dto.Quantity,
                UnitPrice = product.Price,
                TotalPrice = product.Price * dto.Quantity
            };
            _db.CartItems.Add(item);
        }

        // Deduct stock after adding to cart
        product.Stock -= dto.Quantity;
        _db.SaveChanges();

        return Ok(new
        {
            Message = "Product added to cart",
            ProductId = product.ProductId,
            product.Name,
            RemainingStock = product.Stock
        });
    }

    // View all items in the merchant's cart
    [HttpGet("view")]
    public IActionResult ViewCart()
    {
        var merchantId = GetMerchantId();

        var items = _db.CartItems
            .Where(x => x.MerchantId == merchantId)
            .Select(x => new
            {
                x.CartItemId,
                x.ProductId,
                x.Quantity,
                x.UnitPrice,
                x.TotalPrice
            })
            .ToList();

        return Ok(items);
    }

    // Remove a product from the cart by ProductId
    [HttpDelete("remove/{productId}")]
    public IActionResult RemoveFromCart(int productId)
    {
        var merchantId = GetMerchantId();
        var item = _db.CartItems.FirstOrDefault(x => x.ProductId == productId && x.MerchantId == merchantId);

        if (item == null) return NotFound("Product not found in cart.");

        // Restore stock back to inventory
        var product = _db.Products.FirstOrDefault(p => p.ProductId == productId && p.MerchantId == merchantId);
        if (product != null)
            product.Stock += item.Quantity;

        _db.CartItems.Remove(item);
        _db.SaveChanges();

        return Ok($"Product (ID={productId}) removed from cart, stock restored.");
    }

    // Clear all items in the cart
    [HttpDelete("clear")]
    public IActionResult ClearCart()
    {
        var merchantId = GetMerchantId();
        var items = _db.CartItems.Where(x => x.MerchantId == merchantId).ToList();

        // Restore stock for all items
        foreach (var item in items)
        {
            var product = _db.Products.FirstOrDefault(p => p.ProductId == item.ProductId && p.MerchantId == merchantId);
            if (product != null)
                product.Stock += item.Quantity;
        }

        _db.CartItems.RemoveRange(items);
        _db.SaveChanges();

        return Ok("Cart cleared, stock restored.");
    }
}

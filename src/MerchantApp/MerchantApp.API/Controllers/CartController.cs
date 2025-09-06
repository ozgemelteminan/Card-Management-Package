using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CardManagement.Shared.DTOs;
using CardManagement.Data;
using CardManagement.Shared.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace MerchantApp.API.Controllers
{
    [ApiController]
    [Route("api/cart")] // Base route for cart operations
    [Authorize] // Requires JWT authentication
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly AppDbContext _db;

        public CartController(ICartService cartService, AppDbContext db)
        {
            _cartService = cartService;
            _db = db;
        }

        private int GetMerchantId() =>
            int.Parse(User.Claims.First(c => c.Type == "merchantId").Value);

        [HttpPost("add")] // Add product to cart
        public async Task<IActionResult> AddToCart([FromBody] CartItemDTO dto)
        {
            var merchantId = GetMerchantId();
            var product = _db.Products.FirstOrDefault(p => p.ProductId == dto.ProductId && p.MerchantId == merchantId);
            if (product == null) return NotFound("Product not found");
            if (product.Stock < dto.Quantity) return BadRequest($"Insufficient stock. Current stock: {product.Stock}");

            dto.UnitPrice = product.Price;
            await _cartService.AddItemAsync(merchantId, dto);

            product.Stock -= dto.Quantity;
            _db.SaveChanges();

            return Ok(new { Message = "Product added to cart", ProductId = product.ProductId, product.Name, RemainingStock = product.Stock });
        }

        [HttpGet("view")] // View all items in cart
        public async Task<IActionResult> ViewCart()
        {
            var merchantId = GetMerchantId();
            var items = await _cartService.GetCartAsync(merchantId);
            return Ok(items.Select(i => new { i.ProductId, i.Quantity, i.UnitPrice, i.TotalPrice }));
        }

        [HttpDelete("remove/{productId}")] // Remove a specific product from cart
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var merchantId = GetMerchantId();
            var cart = await _cartService.GetCartAsync(merchantId);
            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            if (item == null) return NotFound("Product not found in cart.");

            var product = _db.Products.FirstOrDefault(p => p.ProductId == productId && p.MerchantId == merchantId);
            if (product != null) product.Stock += item.Quantity;

            await _cartService.RemoveItemAsync(merchantId, productId);
            _db.SaveChanges();

            return Ok($"Product (ID={productId}) removed from cart, stock restored.");
        }

        [HttpDelete("clear")] // Clear all items in cart
        public async Task<IActionResult> ClearCart()
        {
            var merchantId = GetMerchantId();
            var cart = await _cartService.GetCartAsync(merchantId);

            foreach (var item in cart)
            {
                var product = _db.Products.FirstOrDefault(p => p.ProductId == item.ProductId && p.MerchantId == merchantId);
                if (product != null) product.Stock += item.Quantity;
            }

            await _cartService.ClearCartAsync(merchantId);
            _db.SaveChanges();

            return Ok("Cart cleared, stock restored.");
        }
    }
}

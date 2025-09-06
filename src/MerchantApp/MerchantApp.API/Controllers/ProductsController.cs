using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CardManagement.Shared.DTOs;
using MerchantApp.Service.Services;
using System.Linq;
using System.Threading.Tasks;

namespace MerchantApp.API.Controllers
{
    [ApiController] 
    [Route("api/products")] // Base route for this controller
    [Authorize] // Requires authentication for all actions
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        // Constructor - injects the IProductService dependency
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // Helper method - extracts merchantId from user claims
        private int GetMerchantId()
        {
            return int.Parse(User.Claims.First(c => c.Type == "merchantId").Value);
        }

        // POST api/products/add
        // Adds a new product or updates stock if the product already exists
        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromBody] ProductCreateDTO dto)
        {
            var merchantId = GetMerchantId(); // Get merchantId from claims
            var product = await _productService.AddProductAsync(merchantId, dto); // Call service

            // Return success message and product details
            return Ok(new
            {
                Message = "Product added or stock updated",
                product.ProductId,
                product.Name,
                product.Stock
            });
        }

        // GET api/products/view
        // Retrieves all products that belong to the authenticated merchant
        [HttpGet("view")]
        public async Task<IActionResult> GetMyProducts()
        {
            var merchantId = GetMerchantId(); // Get merchantId from claims
            var products = await _productService.GetProductsByMerchantAsync(merchantId); // Fetch products

            // Return a projection (only selected fields)
            return Ok(products.Select(p => new
            {
                p.ProductId,
                p.Name,
                p.Price,
                p.Stock,
                p.CreatedAt
            }));
        }

        // DELETE api/products/{id}
        // Deletes a product if it belongs to the authenticated merchant
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var merchantId = GetMerchantId(); // Get merchantId from claims
            var success = await _productService.DeleteProductAsync(merchantId, id); // Attempt delete

            if (!success)
                return NotFound("Product not found or does not belong to you."); // Error case

            return Ok("Product deleted."); // Success case
        }
    }
}

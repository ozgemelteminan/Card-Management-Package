using CardManagement.Data;
using CardManagement.Data.Entities;
using Merchant.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Merchant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly CardManagementDbContext _db;
        public ProductController(CardManagementDbContext db) => _db = db;

        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Name) || request.Price <= 0)
                return BadRequest("Geçersiz ürün bilgisi.");

            var entity = new ProductEntity
            {
                // ❌ ProductId = _nextId++ (KALDIRILDI — DB otomatik verecek)
                MerchantId = 1, // şimdilik sabit
                Name = request.Name,
                Price = request.Price,
                CreatedAt = DateTime.UtcNow
            };

            _db.Products.Add(entity);
            await _db.SaveChangesAsync(); // ⬅️ ID burada set edilir

            var dto = new ProductDto
            {
                ProductId = entity.ProductId,
                Name = entity.Name,
                Price = entity.Price,
                CreatedAt = entity.CreatedAt
            };

            return Ok(dto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var list = await _db.Products.AsNoTracking()
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var p = await _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == id);
            if (p == null)
                return NotFound("Ürün bulunamadı.");

            return Ok(new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                CreatedAt = p.CreatedAt
            });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
        {
            var entity = await _db.Products.FirstOrDefaultAsync(x => x.ProductId == id);
            if (entity == null)
                return NotFound("Ürün bulunamadı.");

            entity.Name = request.Name;
            entity.Price = request.Price;

            await _db.SaveChangesAsync();

            return Ok(new ProductDto
            {
                ProductId = entity.ProductId,
                Name = entity.Name,
                Price = entity.Price,
                CreatedAt = entity.CreatedAt
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.Pr_

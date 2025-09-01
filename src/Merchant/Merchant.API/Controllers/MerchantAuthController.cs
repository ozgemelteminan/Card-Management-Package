using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;                  // EF
using CardManagement.Data;                            // DbContext
using CardManagement.Data.Entities;
using Merchant.API.Models;

namespace Merchant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchantAuthController : ControllerBase
    {
        private readonly CardManagementDbContext _db;
        public MerchantAuthController(CardManagementDbContext db) => _db = db;

        [HttpPost("register")]
        public async Task<ActionResult<MerchantDto>> Register([FromBody] RegisterMerchantRequest request)
        {
            if (request is null)
                return BadRequest("Lütfen tüm alanları doldurun.");

            var exists = await _db.Merchants.AnyAsync(m => m.Email == request.Email);
            if (exists) return Conflict("Bu email zaten kayıtlı.");

            var entity = new MerchantEntity
            {
               
                Name = request.Name,
                Email = request.Email,
                PasswordHash = request.Password,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _db.Merchants.Add(entity);
            await _db.SaveChangesAsync();

            var dto = new MerchantDto
            {
                MerchantId = entity.MerchantId,
                Name = entity.Name,
                Email = entity.Email,
                CreatedAt = entity.CreatedAt
            };

            return Ok(dto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request is null)
                return BadRequest("Lütfen alanları doldurunuz.");

            var merchant = await _db.Merchants.FirstOrDefaultAsync(m => m.Email == request.Email);

            if (merchant == null)
                return NotFound("Bu E-Mail ile kullanıcı yok.");

            if (merchant.PasswordHash != request.Password)
                return BadRequest("Hatalı şifre girdiniz.");

            var dto = new MerchantDto
            {
                MerchantId = merchant.MerchantId,
                Name = merchant.Name,
                Email = merchant.Email,
                CreatedAt = merchant.CreatedAt
            };

            return Ok(new { message = "Giriş başarılı", merchant = dto });
        }
    }
}

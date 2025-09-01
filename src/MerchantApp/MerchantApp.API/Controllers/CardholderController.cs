using Microsoft.AspNetCore.Mvc;
using MerchantApp.API.Data;
using MerchantApp.API.Models;
using MerchantApp.API.DTOs;

namespace MerchantApp.API.Controllers
{
    [ApiController]
    [Route("api/cardholders")]
    public class CardholdersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CardholdersController(AppDbContext db) => _db = db;

        // Create a new Cardholder
        [HttpPost("create")]
        public IActionResult CreateCardholder([FromBody] CardholderCreateDTO dto)
        {
            // Simple password hashing (for demo only)
            // In production, use BCrypt or SHA256 with salt instead
            var passwordHash = Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(dto.PasswordHash)
            );

            var cardholder = new Cardholder
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = passwordHash
            };

            _db.Cardholders.Add(cardholder);
            _db.SaveChanges();

            // Return safe response DTO (without password hash)
            return Ok(new CardholderResponseDTO
            {
                CardholderId = cardholder.CardholderId,
                FullName = cardholder.FullName,
                Email = cardholder.Email,
                CreatedAt = cardholder.CreatedAt
            });
        }

        // Get all cardholders
        [HttpGet]
        public IActionResult GetAll()
        {
            var cardholders = _db.Cardholders.Select(c => new CardholderResponseDTO
            {
                CardholderId = c.CardholderId,
                FullName = c.FullName,
                Email = c.Email,
                CreatedAt = c.CreatedAt
            }).ToList();

            return Ok(cardholders);
        }
    }
}

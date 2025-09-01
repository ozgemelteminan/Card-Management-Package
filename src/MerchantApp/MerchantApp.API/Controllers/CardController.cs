using Microsoft.AspNetCore.Mvc;
using MerchantApp.API.Data;
using MerchantApp.API.Models;
using MerchantApp.API.DTOs;

namespace MerchantApp.API.Controllers
{
    [ApiController]
    [Route("api/cards")]
    public class CardsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CardsController(AppDbContext db) => _db = db;

        // Create a new card
        [HttpPost("create")]
        public IActionResult CreateCard([FromBody] CardCreateDTO dto)
        {
            // Check if the referenced cardholder exists
            var cardholder = _db.Cardholders.FirstOrDefault(c => c.CardholderId == dto.CardholderId);
            if (cardholder == null) 
                return NotFound("Cardholder not found");

            // Map DTO to Card entity
            var card = new Card
            {
                CardholderId = dto.CardholderId,
                CardNumber = dto.CardNumber,
                ExpiryDate = dto.ExpiryDate,
                CVV = dto.CVV,
                Pin = dto.Pin,
                Balance = dto.Balance
            };

            // Save to database
            _db.Cards.Add(card);
            _db.SaveChanges();

            // Do not return sensitive fields like PIN and CVV
            return Ok(new
            {
                card.CardId,
                card.CardholderId,
                card.CardNumber,
                card.ExpiryDate,
                card.Balance
            });
        }

        // Get all cards (masked response, no sensitive data)
        [HttpGet]
        public IActionResult GetAll()
        {
            var cards = _db.Cards.Select(card => new
            {
                card.CardId,
                card.CardholderId,
                card.CardNumber,
                card.ExpiryDate,
                card.Balance
            }).ToList();

            return Ok(cards);
        }
    }
}

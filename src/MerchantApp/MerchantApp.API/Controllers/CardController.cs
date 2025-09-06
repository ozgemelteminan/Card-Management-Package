using Microsoft.AspNetCore.Mvc;
using CardManagement.Shared.DTOs;   
using CardManagement.Shared.Models;   
using MerchantApp.Service.Services; 

namespace MerchantApp.API.Controllers
{
    [ApiController] 
    [Route("api/cards")] // All endpoints in this controller will be under /api/cards
    public class CardsController : ControllerBase
    {
        private readonly ICardService _cardService; // Service for card-related operations

        public CardsController(ICardService cardService)
        {
            _cardService = cardService;
        }

        // POST: api/cards/create
        // Creates a new card
        [HttpPost("create")]
        public async Task<IActionResult> CreateCard([FromBody] CardCreateDTO dto)
        {
            try
            {
                // Create a card through the service
                var card = await _cardService.CreateCardAsync(dto);

                // Return the created card information
                return Ok(new
                {
                    card.CardId,
                    card.CardholderId,
                    card.CardNumber,
                    card.ExpiryDate,
                    card.Balance
                });
            }
            catch (KeyNotFoundException ex) // If the cardholder is not found
            {
                return NotFound(ex.Message); // Return 404 Not Found
            }
        }

        // GET: api/cards
        // Retrieves all cards
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Get all cards from the service
            var cards = await _cardService.GetAllCardsAsync();

            // Return cards in JSON format
            return Ok(cards.Select(card => new
            {
                card.CardId,
                card.CardholderId,
                card.CardNumber,
                card.ExpiryDate,
                card.Balance
            }));
        }
    }
}

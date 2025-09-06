using Microsoft.AspNetCore.Mvc;
using CardManagement.Shared.DTOs;       
using MerchantApp.Service.Services;     

namespace MerchantApp.API.Controllers
{
    [ApiController] 
    [Route("api/cardholders")] // Base route: /api/cardholders
    public class CardholdersController : ControllerBase
    {
        private readonly ICardholderService _cardholderService; // Service dependency for cardholder operations

        public CardholdersController(ICardholderService cardholderService)
        {
            _cardholderService = cardholderService;
        }

        // POST: api/cardholders/create
        // Creates a new cardholder
        [HttpPost("create")]
        public async Task<IActionResult> CreateCardholder([FromBody] CardholderCreateDTO dto)
        {
            if (!ModelState.IsValid) // Validate request model
                return BadRequest(ModelState);

            try
            {
                // Call service to create a new cardholder
                var cardholder = await _cardholderService.CreateCardholderAsync(dto);

                // Return created cardholder data
                return Ok(cardholder);
            }
            catch (Exception ex) // Handle unexpected errors
            {
                return StatusCode(500, new { message = "Cardholder creation failed.", error = ex.Message });
            }
        }

        // GET: api/cardholders
        // Retrieves all cardholders
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Get cardholders from service
            var cardholders = await _cardholderService.GetAllCardholdersAsync();

            // Return as JSON
            return Ok(cardholders);
        }
    }
}
